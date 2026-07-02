using SSF.Shared.DTOs;
using SSF.Shared;
namespace SSF.Domain.Interfaces
{
    public interface IAuthService
    {
        // Procesa las credenciales y devuelve el DTO con el Token, o null si falla
        Task<GenericResponse<LoginResponseDto?>> LoginAsync(LoginRequestDto request);

        // Nuevo método: Devuelve true si se creó bien, false si el usuario ya existe
        Task<GenericResponse<bool>> RegisterAsync(RegisterRequestDto request);
    }
}