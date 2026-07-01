namespace SSF.Domain.Entities
{
    public class Caja
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty; // Ej: "Caja 01", "Caja Central"
        public bool Activa { get; set; } = true;
        public string? DeviceToken { get; set; }

        // Clave Foránea: Cada caja pertenece obligatoriamente a una Sucursal
        public int SucursalId { get; set; }

        // Propiedad de navegación hacia la Sucursal
        public Sucursal? Sucursal { get; set; }
    }
}