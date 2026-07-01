using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSF.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Acá se guarda el texto encriptado con Salt (lo manejará SSF.Identity)
        public string PasswordHash { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Relación con el Rol (Clave foránea)
        public int RolId { get; set; }
        public Rol Rol { get; set; } = null!;

    }
}
