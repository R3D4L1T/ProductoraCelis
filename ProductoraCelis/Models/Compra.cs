using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductoraCelis.Models
{
    public class Compra
    {
        internal int IdDetalleCompra;

        public int IdCompra { get; set; }
        public DateTime FechaCompra { get; set; }
        [Precision(10, 2)]
        public decimal Total { get; set; }


        public int IdUsuarioCliente { get; set; }
        public Usuario? UsuarioCliente { get; set; }
        public ICollection<DetalleCompra>? DetallesCompra { get; set; } = new List<DetalleCompra>();
        public ICollection<Reporte>? Reportes { get; set; }
        public ICollection<Comprobante>? Comprobantes { get; set; }
        public Pago? Pago { get; set; }
    }

}
