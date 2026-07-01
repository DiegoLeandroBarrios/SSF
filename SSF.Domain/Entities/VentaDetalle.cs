namespace SSF.Domain.Entities
{
    public class VentaDetalle
    {
        public int Id { get; set; }
        public int VentaId { get; set; }
        public int ProductoId { get; set; }
        public decimal Cantidad { get; set; } // Mantiene los 3 decimales para gramos
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }

        // Propiedades de navegación
        public Venta? Venta { get; set; }
        public Producto? Producto { get; set; }
    }
}