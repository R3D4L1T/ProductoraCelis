using System.ComponentModel.DataAnnotations;

namespace ProductoraCelis.Models
{
    public class Clientes
    {
        public int IdClientes { get; set; }
        public string Nombres { get; set; }

        public string Apellidos { get; set; }
        public string Dni { get; set; }
        public string Celular { get; set; }

        public string Direccion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Descripcion { get; set; }
        public bool Estado { get; set; }
        public string Email { get; set; }
    }
}
