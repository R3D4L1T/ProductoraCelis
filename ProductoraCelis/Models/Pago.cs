namespace ProductoraCelis.Models
{
    public class Pago
    {
        public int IdPago { get; set; }

        public int IdCompra { get; set; }
        public Compra? Compra { get; set; }

        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public string? MetodoPago { get; set; }

        public string? NumeroTarjeta { get; set; }
        public string? NombreTitular { get; set; }
        public DateTime? FechaExpiracion { get; set; }
        public string? Cvv { get; set; }
    }
}
