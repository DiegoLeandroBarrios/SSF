namespace SSF.Domain.Entities
{
    public class Sucursal
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Direccion { get; set; }
        public bool Activa { get; set; } = true;

        // Propiedad de navegación: Una sucursal puede tener muchos usuarios
        public ICollection<Caja> Cajas { get; set; } = new List<Caja>();
    }
}