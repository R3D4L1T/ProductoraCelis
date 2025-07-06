using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using ProductoraCelis.Data;
using ProductoraCelis.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebasUnitarias_ProductoraCelis
{
    public class RegistrarProducto
    {
        private readonly AdminController _adminController;
        private readonly pgDbContext _dbContext;

        public RegistrarProducto()
        {
            var options = new DbContextOptionsBuilder<pgDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _dbContext = new pgDbContext(options);
            _adminController = new AdminController(_dbContext);
        }
        [Fact]
        public async Task Registrar_Producto()
        {
            var producto = new Producto
            {
                Nombre = "Leche",
                Categoria = "Lácteos",
                Descripcion = "Leche fresca",
                Stock = 10,
                FechaProduccion = DateTime.Now.AddDays(-1),
                FechaVencimiento = DateTime.Now.AddDays(10),
                Precio = 4.50M,
                Estado = true
            };

            var content = "fake image content";
            var fileName = "leche.jpg";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            var formFileMock = new Mock<IFormFile>();
            formFileMock.Setup(f => f.FileName).Returns(fileName);
            formFileMock.Setup(f => f.Length).Returns(stream.Length);
            formFileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            formFileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
                        .Returns<Stream, System.Threading.CancellationToken>((target, token) => stream.CopyToAsync(target));

            var result = await _adminController.Registrar_Producto(producto, formFileMock.Object);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Dashboard", redirect.ActionName);

            var productoDb = await _dbContext.Productos.FirstOrDefaultAsync(p => p.Nombre == "Leche");
            Assert.NotNull(productoDb);
            Assert.Contains("/images/", productoDb.UrlImagen);
        }
        [Fact]
        public async Task Editar_Producto()
        {
            var options = new DbContextOptionsBuilder<pgDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // usar una base única por test
                .Options;

            using var context = new pgDbContext(options);

            var productoExistente = new Producto
            {
                IdProducto = 100,
                Nombre = "Producto A",
                Descripcion = "Desc A",
                Categoria = "leche",
                Precio = 100
            };
            context.Productos.Add(productoExistente);
            await context.SaveChangesAsync();

            var controller = new AdminController(context);

            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            controller.TempData = tempData;

            var productoActualizado = new Producto
            {
                IdProducto = 100,
                Nombre = "Producto Actualizado",
                Descripcion = "Nueva desc",
                Categoria = "leche",
                Precio = 150
            };

            var result = await controller.Editar_Producto(productoActualizado, null);

            var productoDb = await context.Productos.FindAsync(100);
            Assert.NotNull(productoDb);
            Assert.Equal("Producto Actualizado", productoDb.Nombre);
            Assert.Equal(150, productoDb.Precio);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Dashboard", redirect.ActionName);
        }

        //Validar los campos del modelo Producto
        private List<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var ctx = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, ctx, validationResults, true);
            return validationResults;
        }

        [Fact]
        public void CamposIncorrectos()
        {
            var producto = new Producto
            {
                Nombre = "Producto X",
                Categoria = "pan", 
                Descripcion = "Descripción",
                Stock = 10,
                FechaProduccion = DateTime.Now,
                FechaVencimiento = DateTime.Now.AddDays(10),
                Precio = 9.99m,
                Estado = true
            };

            var errores = ValidateModel(producto);

            Assert.Contains(errores, e => e.ErrorMessage.Contains("Categoría no válida"));
        }

        [Fact]
        public void CamposCorrectas()
        {
            var producto = new Producto
            {
                Nombre = "Leche Entera",
                Categoria = "leche",
                Descripcion = "Producto lácteo",
                Stock = 100,
                FechaProduccion = DateTime.Now,
                FechaVencimiento = DateTime.Now.AddMonths(1),
                Precio = 5.99m,
                Estado = true
            };

            var errores = ValidateModel(producto);

            Assert.Empty(errores);
        }
        [Fact]
        public async Task Eliminar_Producto()
        {
            var options = new DbContextOptionsBuilder<pgDbContext>()
                .UseInMemoryDatabase("EliminarProducto")
                .Options;

            using var context = new pgDbContext(options);
            context.Productos.Add(new Producto
            {
                IdProducto = 1000,
                Nombre = "Producto A",
                Descripcion = "Desc A",
                Categoria = "leche",
                Precio = 100
            });
            await context.SaveChangesAsync();

            var controller = new AdminController(context);
            var resultado = await controller.Eliminar_Producto(1000);

            var productoExiste = context.Productos.Any(p => p.IdProducto == 1000);
            Assert.False(productoExiste);
        }

    }
}
    
