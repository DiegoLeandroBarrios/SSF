using Microsoft.AspNetCore.Identity;
using SSF.Domain.Interfaces;

namespace SSF.Identity.Services
{
    public class PasswordHasherService : IPasswordHasher
    {
        // Usamos un tipo dummy (string) para el componente genérico nativo
        private readonly PasswordHasher<string> _hasher = new();

        public string HashPassword(string password)
        {
            // El primer parámetro es el usuario (no es obligatorio, pasamos un string vacío)
            return _hasher.HashPassword(string.Empty, password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            var result = _hasher.VerifyHashedPassword(string.Empty, hashedPassword, password);

            // Si el resultado es Success, la contraseña es correcta
            return result == PasswordVerificationResult.Success;
        }
    }
}