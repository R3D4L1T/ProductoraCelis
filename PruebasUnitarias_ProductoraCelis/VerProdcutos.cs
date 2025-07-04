using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ProductoraCelis.Controllers;
using ProductoraCelis.Data;
using ProductoraCelis.Models;
using ProductoraCelis.ViewsModes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PruebasUnitarias_ProductoraCelis
{
    public class VerProdcutos
    {
        [Fact]
        public async Task ClienteVer_Prodcutos()
        {
            var mockLogger = new Mock<ILogger<HomeController>>();

            var options = new DbContextOptionsBuilder<pgDbContext>()
                .UseInMemoryDatabase(databaseName: "ProductosTestDb")
                .Options;

            using (var context = new pgDbContext(options))
            {
                context.Productos.AddRange(
                    new Producto
                    {
                        IdProducto = 200,
                        Nombre = "Producto a",
                        Descripcion = "Desc a",
                        Categoria = "leche",
                        Precio = 100
                    },

                    new Producto
                    {
                        IdProducto = 201,
                        Nombre = "Producto b",
                        Descripcion = "Desc b",
                        Categoria = "leche",
                        Precio = 101
                    }
                );
                context.SaveChanges();

                var controller = new HomeController(mockLogger.Object, context);

                var result = await controller.Productos();

                var viewResult = Assert.IsType<ViewResult>(result);
                var model = Assert.IsAssignableFrom<List<Producto>>(viewResult.Model);
                Assert.Equal(2, model.Count);
                Assert.Contains(model, p => p.Nombre == "Producto a");
                Assert.Contains(model, p => p.Nombre == "Producto b");
            }
        }
        [Fact]
        public void Realizar_Pago()
        {
            var fechaPago = DateTime.Now;
            var fechaExpiracion = new DateTime(2026, 12, 1);

            var pago = new Pago
            {
                IdPago = 1,
                IdCompra = 100,
                Monto = 150.50m,
                FechaPago = fechaPago,
                MetodoPago = "Tarjeta",
                NumeroTarjeta = "1234567890123456",
                NombreTitular = "José Soto",
                FechaExpiracion = fechaExpiracion,
                Cvv = "123"
            };
            Assert.Equal(1, pago.IdPago);
            Assert.Equal(100, pago.IdCompra);
            Assert.Equal(150.50m, pago.Monto);
            Assert.Equal(fechaPago, pago.FechaPago);
            Assert.Equal("Tarjeta", pago.MetodoPago);
            Assert.Equal("1234567890123456", pago.NumeroTarjeta);
            Assert.Equal("José Soto", pago.NombreTitular);
            Assert.Equal(fechaExpiracion, pago.FechaExpiracion);
            Assert.Equal("123", pago.Cvv);
        }
        [Fact]
        public void ProductoAgregar_Carrito()
        {
            var carritoItems = new List<Carrito>();
            var fechaHora = DateTime.Now;

            var producto = new Carrito
            {
                IdCarrito = 1,
                UsuarioId = "usuario123",
                IdProducto = 101,
                Nombre = "Leche Descremada",
                Categoria = "leche",
                Precio = 2499.99m,
                Cantidad = 2,
                UrlImagen = "/imges/leche.jpg",
                FechaHora = fechaHora
            };

            carritoItems.Add(producto);

            Assert.Single(carritoItems);

            var item = carritoItems[0];
            Assert.Equal(1, item.IdCarrito);
            Assert.Equal("usuario123", item.UsuarioId);
            Assert.Equal(101, item.IdProducto);
            Assert.Equal("Leche Descremada", item.Nombre);
            Assert.Equal("leche", item.Categoria);
            Assert.Equal(2499.99m, item.Precio);
            Assert.Equal(2, item.Cantidad);
            Assert.Equal("/imges/leche.jpg", item.UrlImagen);
            Assert.Equal(fechaHora, item.FechaHora);
        }
        [Fact]
        public void HistorialVentas()
        {
            var carrito = new List<Carrito>
            {
                new Carrito
                {
                    IdCarrito = 1,
                    UsuarioId = "user123",
                    IdProducto = 10,
                    Nombre = "Producto A",
                    Categoria = "Categoria 1",
                    Precio = 50.0m,
                    Cantidad = 2,
                    UrlImagen = "imagen1.jpg",
                    FechaHora = DateTime.Now
                },
                new Carrito
                {
                    IdCarrito = 2,
                    UsuarioId = "user123",
                    IdProducto = 20,
                    Nombre = "Producto B",
                    Categoria = "Categoria 2",
                    Precio = 100.0m,
                    Cantidad = 1,
                    UrlImagen = "imagen2.jpg",
                    FechaHora = DateTime.Now
                }
            };
            decimal subtotalProducto1 = carrito[0].Precio * carrito[0].Cantidad;
            decimal subtotalProducto2 = carrito[1].Precio * carrito[1].Cantidad; 
            decimal totalCarrito = carrito.Sum(p => p.Precio * p.Cantidad); 

            Assert.Equal(2, carrito.Count);
            Assert.Equal("Producto A", carrito[0].Nombre);
            Assert.Equal("Categoria 1", carrito[0].Categoria);
            Assert.Equal(2, carrito[0].Cantidad);
            Assert.Equal(100m, subtotalProducto1);

            Assert.Equal("Producto B", carrito[1].Nombre);
            Assert.Equal("Categoria 2", carrito[1].Categoria);
            Assert.Equal(1, carrito[1].Cantidad);
            Assert.Equal(100m, subtotalProducto2);

            Assert.Equal(200m, totalCarrito);
        }


    }

}
