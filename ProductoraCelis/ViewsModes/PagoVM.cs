using ProductoraCelis.Models;

namespace ProductoraCelis.ViewsModes
{
    public class PagoVM
    {
        public int IdUsuarioCliente { get; set; }
        public int IdCompra { get; set; }  // para enviar el idCompra al js
        public List<Carrito> Carrito { get; set; } = new List<Carrito>();

        public decimal Total { get; set; }
        public string? MetodoPago { get; set; }
        public string? NumeroTarjeta { get; set; }
        public string? NombreTitular { get; set; }
        public DateTime? FechaExpiracion { get; set; }
        public string? Cvv { get; set; }
    }
}
