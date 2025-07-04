using ProductoraCelis.Models;

namespace ProductoraCelis.ViewsModes
{
    public class ComprobanteVM
    {
        public int IdCompra { get; set; }
        public string MetodoPago { get; set; }
        public decimal Total { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public List<Carrito> Carrito { get; set; } = new List<Carrito>();
    }
}
