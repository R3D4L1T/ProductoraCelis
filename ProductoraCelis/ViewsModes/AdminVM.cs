using ProductoraCelis.Models;

namespace ProductoraCelis.ViewsModes
{
    public class AdminVM
    {
        public int TotalClientes { get; set; }
        public int TotalProveedores { get; set; }
        public int TotalProductos { get; set; }
        public List<ProductoraCelis.Models.Clientes> Clientes { get; set; }
        public List<ProductoraCelis.Models.Proveedor> Proveedores { get; set; }
        public List<ProductoraCelis.Models.Producto> Productos { get; set; }
    }
}

