namespace SSF.Shared.DTOs
{
    public class RegisterRequestDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RolId { get; set; } // 1 para Administrador, 2 para Cajero
        public int SucursalId { get; set; } // Por ahora mandamos un ID temporal (ej: 1)
    }
}