using System.ComponentModel.DataAnnotations;

namespace ProductoraCelis.Models
{
    public class Producto
    {
        public int IdProducto { get; set; }

        public string Nombre { get; set; }
        [Required]
        [RegularExpression("leche|queso|yogurt|mantequilla", ErrorMessage = "Categoría no válida")]
        public string Categoria { get; set; }

        public string Descripcion { get; set; }
        public int Stock { get; set; }


        public DateTime FechaProduccion { get; set; }

        public DateTime FechaVencimiento { get; set; }

        public decimal Precio { get; set; }

        public bool Estado { get; set; }
        public String? UrlImagen { get; set; }
        public ICollection<Reporte> ReporteVenta { get; set; }
    }
}
