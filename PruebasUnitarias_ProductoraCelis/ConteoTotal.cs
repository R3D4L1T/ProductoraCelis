using ProductoraCelis.Models;
using ProductoraCelis.ViewsModes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebasUnitarias_ProductoraCelis
{
    public class ConteoTotal
    {
        [Fact]
        public void Conteo_Clientes_Proveedores_Productos()
        {
            // Arrange
            var adminVM = new AdminVM();

            // Act - asignar valores simples
            adminVM.TotalClientes = 5;
            adminVM.TotalProveedores = 3;
            adminVM.TotalProductos = 10;

            // Act - crear listas con datos de prueba
            adminVM.Clientes = new List<Clientes>
            {
                new Clientes { IdClientes = 1, Nombres = "Cliente1" },
                new Clientes { IdClientes = 2, Nombres = "Cliente2" }
            };

            adminVM.Proveedores = new List<Proveedor>
            {
                new Proveedor { IdProveedor = 1, Nombres = "Proveedor1" }
            };

            adminVM.Productos = new List<Producto>
            {
                new Producto { IdProducto = 1, Nombre = "Producto1", Precio = 100 },
                new Producto { IdProducto = 2, Nombre = "Producto2", Precio = 200 }
            };

            // Assert
            Assert.Equal(5, adminVM.TotalClientes);
            Assert.Equal(3, adminVM.TotalProveedores);
            Assert.Equal(10, adminVM.TotalProductos);

            Assert.NotNull(adminVM.Clientes);
            Assert.Equal(2, adminVM.Clientes.Count);
            Assert.Equal("Cliente1", adminVM.Clientes[0].Nombres);

            Assert.NotNull(adminVM.Proveedores);
            Assert.Single(adminVM.Proveedores);
            Assert.Equal("Proveedor1", adminVM.Proveedores[0].Nombres);

            Assert.NotNull(adminVM.Productos);
            Assert.Equal(2, adminVM.Productos.Count);
            Assert.Equal(100, adminVM.Productos[0].Precio);
        }
    }
}
