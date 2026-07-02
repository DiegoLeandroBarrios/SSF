using Microsoft.EntityFrameworkCore;
using SSF.Data.Context;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SSF.Identity.Services; // Referencia a tus servicios de criptografía
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SSF.Domain.Interfaces;
using Serilog;
using Serilog.Events;
using Microsoft.AspNetCore.Builder;

#region Serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) //comment this to log asp.net library information
    .MinimumLevel.Override("System", LogEventLevel.Warning) //comment this to log .Net library information
    .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} : {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("Logs/SSFlogs.txt",
                  rollingInterval: RollingInterval.Day,
                  outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

#endregion

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog(); // Reemplaza el logger por defecto de .NET con Serilog

var connectionString = builder.Configuration.GetConnectionString("CadenaSQL");

// Inyectamos nuestro ApplicationDbContext en el contenedor de servicios
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, b =>
        b.MigrationsAssembly("SSF.Data") // Le avisamos que las Migrations se guardan en SSF.Data
    ));


#region JWT

// INYECCIÓN DE DEPENDENCIAS DE IDENTIDAD
// Enlazamos las interfaces del Dominio con las implementaciones de Identity
builder.Services.AddScoped<IPasswordHasher, PasswordHasherService>();
builder.Services.AddScoped<IJwtProvider, JwtProviderService>();
builder.Services.AddScoped<IAuthService, AuthService>();

//CONFIGURACIÓN DEL MIDDLEWARE DE AUTENTICACIÓN (JWT)
var secretKey = builder.Configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("Falta la Clave Secreta de JWT.");
var key = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    // Establecemos que por defecto el sistema use JWT para autenticar
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Cambiar a true en producción definitiva
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),

        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],

        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],

        ValidateLifetime = true, // Verifica que el token no esté vencido
        ClockSkew = TimeSpan.Zero // Elimina el tiempo de gracia de 5 minutos por defecto
    };
});

#endregion

#region Swagger
// Modificamos el AddSwaggerGen para que configure el candado de seguridad
builder.Services.AddSwaggerGen(options =>
{
    // Definimos el esquema de seguridad que va a usar Swagger (JWT Bearer)
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Pegá tu Token JWT acá adentro con este formato: Bearer {tu_token}"
    });

    // Le decimos a Swagger que aplique este requisito de seguridad a todos los endpoints
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

#endregion

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Sumamos esto para que nos habilite la pantalla de pruebas gráfica
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // Esto le dice a Swagger dónde ir a leer el JSON que genera .NET 9
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SSF API V1"); ;
        options.RoutePrefix = "swagger"; // Forzamos que la ruta sea /swagger
    });
}

app.UseHttpsRedirection();

// ACTIVACIÓN DE LA SEGURIDAD EN EL PIPELINE
app.UseAuthentication(); // MUY IMPORTANTE!: Va primero. Identifica quién es el usuario.
app.UseAuthorization();  // Va segundo. Verifica si su Rol tiene permiso.

app.MapControllers();

app.Run();
