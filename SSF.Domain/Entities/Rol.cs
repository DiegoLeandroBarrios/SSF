using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSF.Domain.Entities
{
    public class Rol
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty; // "Administrador", "Cajero"

        // Propiedad de navegación inversa para Entity Framework (Un rol tiene muchos usuarios)
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}