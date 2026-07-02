using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SSF.Domain.Entities;
using SSF.Domain.Interfaces;

namespace SSF.Identity.Services
{
    public class JwtProviderService : IJwtProvider
    {
        private readonly IConfiguration _configuration;

        // Inyectamos IConfiguration para poder leer las variables del appsettings.json
        public JwtProviderService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Generate(Usuario usuario)
        {
            //Creamos las tarjetas de identificación (Claims) del usuario
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Username),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol.Nombre), // ¡Clave para los permisos!
            };

            //Levantamos la clave secreta y la transformamos en bytes criptográficos
            var secretKey = _configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("Falta la Clave Secreta de JWT.");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            //Definimos el algoritmo de firma (HMAC SHA256)
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //Armamos el cuerpo del Token
            var token = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:DurationInMinutes"] ?? "60")),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = credentials
            };

            //Lo procesamos y lo escupimos como un string limpio
            var tokenHandler = new JwtSecurityTokenHandler();
            var stringToken = tokenHandler.CreateToken(token);

            return tokenHandler.WriteToken(stringToken);
        }
    }
}