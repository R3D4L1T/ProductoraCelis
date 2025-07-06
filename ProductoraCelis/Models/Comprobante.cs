namespace ProductoraCelis.Models
{
    public class Comprobante
    {
        public int IdComprobante { get; set; }
        public int IdCompra { get; set; }
        public byte[] ArchivoPdf { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public string RutaArchivo { get; set; }
        public Compra Compra { get; set; }
    }
}
