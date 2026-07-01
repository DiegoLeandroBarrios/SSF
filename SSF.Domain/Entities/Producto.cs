namespace SSF.Domain.Entities
{
    public class Producto
    {
        public int Id { get; set; }
        public string? CodigoBarra { get; set; }
        public string? CodigoPLU { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal PrecioVenta { get; set; }
        public bool ManejaPeso { get; set; } = true;
        public string? ProveedorTexto { get; set; }
    }
}