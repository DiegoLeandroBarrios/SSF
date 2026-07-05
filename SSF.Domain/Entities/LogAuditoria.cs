using System;

namespace SSF.Domain.Entities
{
    public class LogAuditoria
    {
        public long Id { get; set; }
        public DateTimeOffset Fecha { get; set; } = DateTimeOffset.UtcNow;

        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; } // Propiedad de navegación

        public int IdSucursal { get; set; }
        public Sucursal Sucursal { get; set; } // Propiedad de navegación

        public string Accion { get; set; }
        public string Detalle { get; set; }
        public string IpDireccion { get; set; }
    }
}