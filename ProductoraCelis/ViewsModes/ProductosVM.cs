namespace ProductoraCelis.ViewsModes
{
    public class ProductosVM
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public string Categoria { get; set; }
        public string UrlImagen { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
    }
    public class AgregarProductosVM
    {
        public List<ProductosVM> Items { get; set; }
    }
}
