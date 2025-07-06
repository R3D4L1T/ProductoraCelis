using System.ComponentModel.DataAnnotations;

namespace ProductoraCelis.Models
{
    public class Proveedor
    {
        public int IdProveedor { get; set; }
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }
        public string? Dni { get; set; }
        public string? Celular { get; set; }
        public string? Direccion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string? Descripcion { get; set; }
        public string? Tipo { get; set; }
        public bool Estado { get; set; }
        public string? Email { get; set; }
    }
}
