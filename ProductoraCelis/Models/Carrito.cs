
namespace ProductoraCelis.Models
{
    public class Carrito
    {
        public int IdCarrito { get; set; }
        public string UsuarioId { get; set; }
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public string Categoria { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public string? UrlImagen { get; set; }
        public DateTime FechaHora { get; set; }
    }
}   
