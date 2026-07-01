namespace SSF.Domain.Entities
{
    public class VentaPago
    {
        public int Id { get; set; }
        public int VentaId { get; set; }
        public int MedioPagoId { get; set; }
        public decimal Monto { get; set; }
        public string? NroReferencia { get; set; } // Opcional (null) para el cupón de tarjeta/QR

        // Propiedades de navegación
        public Venta? Venta { get; set; }
        public MedioPago? MedioPago { get; set; }
    }
}