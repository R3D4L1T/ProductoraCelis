using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProductoraCelis.Controllers;
using ProductoraCelis.Data;
using ProductoraCelis.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;

namespace Pruebas_Unitarias
{
    [TestClass]
    public class Test1
    {
        private HomeController _controller;
        private pgDbContext _dbContext;
        private Mock<ILogger<HomeController>> _mockLogger;

        [TestInitialize]
        public void Inicializar()
        {
            _mockLogger = new Mock<ILogger<HomeController>>();

            // Agregar la referencia al paquete NuGet Microsoft.EntityFrameworkCore.InMemory si aún no está instalada.
            // Esto habilita el uso de UseInMemoryDatabase.

            var options = new DbContextOptionsBuilder<pgDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb") // Esto requiere el paquete Microsoft.EntityFrameworkCore.InMemory
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

        [TestMethod]
        public void ObtenerClientes_DeberiaRetornarLista()
        {
            var resultado = _dbContext.Clientes.ToList();

            Assert.IsNotNull(resultado);
            Assert.AreEqual(2, resultado.Count);
        }
    }
}
