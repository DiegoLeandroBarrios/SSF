using Microsoft.EntityFrameworkCore;
using SSF.Data.Context;
using SSF.Domain.Entities;
using SSF.Domain.Interfaces;
using SSF.Shared.DTOs;
using SSF.Shared;
using Microsoft.Extensions.Logging;
namespace SSF.Identity.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;
        private readonly ILogger<AuthService> _log;

        public AuthService(
            ApplicationDbContext context,
            IPasswordHasher passwordHasher,
            IJwtProvider jwtProvider, ILogger<AuthService> log)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
            _log = log;
        }

        public async Task<GenericResponse<LoginResponseDto?>> LoginAsync(LoginRequestDto request)
        {
            var response = new GenericResponse<LoginResponseDto?>();
            try
            {
                #region Validaciones

                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    _log.LogDebug("Login fallido: El campo Username está vacío.");
                    response.Success = false;
                    response.Message = "El nombre de usuario es obligatorio.";
                    response.Status = (int)StatusCodeType.ValidationError;

                    return response;
                }
                if (string.IsNullOrWhiteSpace(request.Password))
                {
                    _log.LogDebug("Login fallido para {Email}: El campo Password está vacío.", request.Email);
                    response.Success = false;
                    response.Message = "La contraseña es obligatoria.";
                    response.Status = (int)StatusCodeType.ValidationError;

                    return response;
                }

                #endregion

                //Buscamos el usuario por su Email e incluimos su Rol
                _log.LogInformation("Inicio de Busqueda de usuario por email: {Email}", request.Email);
                var usuario = await _context.Usuarios
                    .Include(u => u.Rol)
                    .FirstOrDefaultAsync(u => u.Email == request.Email);

                //Si no existe o está desactivado, rebotamos con un null genérico (por seguridad)
                if (usuario == null || !usuario.Activo)
                {
                    _log.LogDebug("Intento de login fallido. Usuario inexistente o inactivo: {Email}", request.Email);
                    response.Success = false;
                    response.Message = "Usuario inexistente o invalidado.";
                    response.Status = (int)StatusCodeType.ValidationError;

                    return response;
                }

                //Verificamos matemáticamente la contraseña
                bool esValida = _passwordHasher.VerifyPassword(request.Password, usuario.PasswordHash);

                if (!esValida)
                {
                    _log.LogDebug("Contraseña incorrecta para el usuario: {Username}", usuario.Username);
                    response.Success = false;
                    response.Message = "Contraseña Invalida.";
                    response.Status = (int)StatusCodeType.ValidationError;

                    return response;
                }

                //Si las credenciales son válidas, generamos el Token JWT
                _log.LogInformation("Validando credenciales...");
                var token = _jwtProvider.Generate(usuario);

                var usuarioDto = new LoginResponseDto
                {
                    Username = usuario.Username,
                    Rol = usuario.Rol?.Nombre ?? "Sin Rol", // Evitamos un NullReferenceException si el Rol viene nulo por error
                    Token = token 
                };

                // 5. Devolvemos la respuesta armada para el mostrador
                _log.LogInformation("Usuario: {Username} encontrado con exito, logueo exitoso!", usuario.Username);
                response.Success = true;
                response.Message = "Logueo Exitoso.";
                response.Data = usuarioDto;
                response.Status = (int)StatusCodeType.Success;

                return response;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error inesperado!");
                response.Success = false;
                response.Message = "Error Inesperado.";
                response.Status = (int)StatusCodeType.UnexpectedError;

                return response;
            }
            
        }
        public async Task<GenericResponse<bool>> RegisterAsync(RegisterRequestDto request)
        {
            var response = new GenericResponse<bool>();
            try
            {
                #region Validaciones

                if (string.IsNullOrWhiteSpace(request.Username))
                {
                    _log.LogDebug("Registro fallido: El campo Username está vacío.");
                    response.Success = false;
                    response.Message = "El nombre de usuario es obligatorio.";
                    response.Status = (int)StatusCodeType.ValidationError;
                    response.Data = false;

                    return response;
                }
                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    _log.LogDebug("Registro fallido para {Username}: El campo Email está vacío.", request.Username);
                    response.Success = false;
                    response.Message = "El correo electrónico es obligatorio.";
                    response.Status = (int)StatusCodeType.ValidationError;
                    response.Data = false;

                    return response;
                }
                if (string.IsNullOrWhiteSpace(request.Password))
                {
                    _log.LogDebug("Registro fallido para {Username}: El campo Password está vacío.", request.Username);
                    response.Success = false;
                    response.Message = "La contraseña es obligatoria.";
                    response.Status = (int)StatusCodeType.ValidationError;
                    response.Data = false;

                    return response;
                }
                if (request.Password != request.ConfirmPassword)
                {
                    _log.LogDebug("Registro fallido para {Username}: Las contraseñas no coinciden.", request.Username);
                    response.Success = false;
                    response.Message = "Las contraseñas no coinciden.";
                    response.Status = (int)StatusCodeType.ValidationError;
                    response.Data = false;

                    return response;
                }
                if (request.RolId <= 0)
                {
                    _log.LogDebug("Registro fallido para {Username}: RolId inválido ({RolId}).", request.Username, request.RolId);
                    response.Success = false;
                    response.Message = "Debe asignar un rol válido.";
                    response.Status = (int)StatusCodeType.ValidationError;
                    response.Data = false;

                    return response;
                }

                #endregion

                // 1. Validamos si ya existe un usuario con ese mismo Username
                _log.LogInformation("Iniciando solicitud de registro para el usuario: {Username}", request.Username);
                var existeUsuario = await _context.Usuarios.AnyAsync(u => u.Email == request.Email);
                if (existeUsuario)
                {
                    _log.LogDebug("No se pudo registrar. El usuario ya existe: {Email}", request.Email);
                    response.Success = false;
                    response.Message = "Usuario Existente.";
                    response.Status = (int)StatusCodeType.ValidationError;
                    response.Data = false;

                    return response;
                }

                // 2. Hasheamos la contraseña que viene en texto plano
                string hash = _passwordHasher.HashPassword(request.Password);

                // 3. Mapeamos el DTO a nuestra Entidad del Dominio
                _log.LogInformation("Añadiendo nuevo Usuario: {Username}", request.Username);
                var nuevoUsuario = new Usuario
                {
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = hash,
                    RolId = request.RolId,
                    Activo = true
                };

                // 4. Lo agregamos al contexto y guardamos los cambios en SQL Server
                await _context.Usuarios.AddAsync(nuevoUsuario);
                var result = await _context.SaveChangesAsync();
                if(result > 0) 
                {
                    _log.LogInformation("Usuario creado correctamente en la base de datos: {Username}", request.Username);
                    response.Success = true;
                    response.Message = "Usuario Creado correctamente.";
                    response.Status = (int)StatusCodeType.Success;
                    response.Data = true;

                }
                else 
                {
                    _log.LogWarning("SaveChangesAsync devolvió 0 al intentar registrar a: {Username}", request.Username);
                    response.Success = false;
                    response.Message = "Error Inesperado";
                    response.Status = (int)StatusCodeType.UnexpectedError;
                    response.Data = false;
                }

                return response;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error inesperado!");
                response.Success = false;
                response.Message = "Error Inesperado";
                response.Status = (int)StatusCodeType.UnexpectedError;
                response.Data = false;

                return response;
            }

        }
    }
}