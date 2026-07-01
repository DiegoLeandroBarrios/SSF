namespace SSF.Domain.Entities
{
    public class StockSucursal
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public int SucursalId { get; set; }
        public decimal Cantidad { get; set; } // Mantiene los 3 decimales para gramos

        // Propiedades de navegación
        public Producto? Producto { get; set; }
        public Sucursal? Sucursal { get; set; }
    }
}