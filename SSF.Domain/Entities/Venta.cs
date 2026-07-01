using System;
using System.Collections.Generic;

namespace SSF.Domain.Entities
{
    public class Venta
    {
        public int Id { get; set; }
        public int CajaId { get; set; }
        public int UsuarioId { get; set; }

        // Usamos DateTimeOffset para coincidir con DATETIMEOFFSET de SQL y manejar zonas horarias de forma segura
        public DateTimeOffset Fecha { get; set; } = DateTimeOffset.Now;

        public decimal Total { get; set; }
        public string RutaPDF { get; set; } = string.Empty;

        // Propiedades de navegación de Seguridad
        public Caja? Caja { get; set; }
        public Usuario? Usuario { get; set; }

        // Propiedades de navegación hacia los módulos que haremos después (Detalles y Pagos)
        public ICollection<VentaDetalle> Detalles { get; set; } = new List<VentaDetalle>();
        public ICollection<VentaPago> Pagos { get; set; } = new List<VentaPago>();
    }
}