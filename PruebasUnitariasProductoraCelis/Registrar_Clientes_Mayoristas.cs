using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Moq;
using ProductoraCelis.Controllers;
using ProductoraCelis.Data;
using ProductoraCelis.Models;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;
namespace PruebasUnitariasProductoraCelis
{

    [TestClass]
    public class Registrar_Clientes_Mayoristas
    {
        private readonly HomeController _controller;
        private readonly pgDbContext _dbContext;
        private readonly Mock<ILogger<HomeController>> _mockLogger;
        public Registrar_Clientes_Mayoristas()
        {
            _mockLogger = new Mock<ILogger<HomeController>>();

            var options = new DbContextOptionsBuilder<pgDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _dbContext = new pgDbContext(options);
            _controller = new HomeController(_mockLogger.Object, _dbContext);
            var clientes = new List<Clientes>
            {
                new Clientes { IdClientes= 1, Nombres = "Fany 1", Apellidos = "Palomino Yupanqui", Dni = "45632145", Celular = "987456312",Direccion= "Nuevo Cajamarca", Descripcion="Cliente de leche", Estado= true, Email="fani1235@hmail.com"},
                new Clientes { IdClientes= 2, Nombres = "Fany 2", Apellidos = "Palomino Yupanqui 2", Dni = "4563215", Celular = "987456302",Direccion= "Cajamarca", Descripcion="Cliente de Quesos", Estado= true, Email="fany@hmail.com"}
            };
            _dbContext.Clientes.AddRange(clientes);
            _dbContext.SaveChanges();

            // Simular usuario autenticado
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }
        [Fact]
        public void ObtenerClientes_DeberiaRetornarLista()
        {
            // Act
            var resultado = _dbContext.Clientes.ToList();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
        }

    }
}
