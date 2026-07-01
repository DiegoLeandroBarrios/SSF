namespace SSF.Domain.Entities
{
    public class MedioPago
    {
        public int Id { get; set; } 
        public string NombreMedio { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
    }
}