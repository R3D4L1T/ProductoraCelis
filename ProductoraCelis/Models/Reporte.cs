namespace ProductoraCelis.Models
{
    public class Reporte
    {
        public int IdReporte { get; set; }

        public int IdComprobante { get; set; }
        public Comprobante Comprobante { get; set; }

        public int IdCompra { get; set; }
        public Compra Compra { get; set; }

        public int IdProducto { get; set; }
        public Producto Producto { get; set; }

        public int TotalCantidadVendida { get; set; }

        public string Categoria { get; set; }

        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public DateTime FechaHora { get; set; }
    }
}
