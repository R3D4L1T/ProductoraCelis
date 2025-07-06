using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ProductoraCelis.Controllers;
using ProductoraCelis.Data;
using ProductoraCelis.Models;
using Xunit;

namespace PruebasUnitarias_ProductoraCelis
{
    public class Registrar_ClientesMayoristas
    {
        private readonly AdminController _adminController;
        private readonly HomeController _homeController;
        private readonly pgDbContext _dbContext;

        public Registrar_ClientesMayoristas()
        {
            var options = new DbContextOptionsBuilder<pgDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new pgDbContext(options);

            _adminController = new AdminController(_dbContext); 
            var mockLogger = new Mock<ILogger<HomeController>>();
            _homeController = new HomeController(mockLogger.Object, _dbContext);

            // Agregar datos de prueba
            var clientes = new List<Clientes>
    {
        new Clientes { IdClientes = 1, Nombres = "Fany 1", Apellidos = "Palomino Yupanqui", Dni = "45632145", Celular = "987456312", Direccion = "Nuevo Cajamarca", Descripcion = "Cliente de leche", Estado = true, Email = "fani1235@hmail.com" },
        new Clientes { IdClientes = 2, Nombres = "Fany 2", Apellidos = "Palomino Yupanqui 2", Dni = "4563215", Celular = "987456302", Direccion = "Cajamarca", Descripcion = "Cliente de Quesos", Estado = true, Email = "fany@hmail.com" }
    };

            _dbContext.Clientes.AddRange(clientes);
            _dbContext.SaveChanges();

            // Simular usuario autenticado
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "1") };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            _adminController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        /*
        [Fact]
        public void ObtenerClientes_PorLista()
        {
            // Act
            var resultado = _dbContext.Clientes.ToList();

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
        }
        */
        [Fact]
        public void EditarCliente_Mayorista()
        {
            var clienteExistente = _dbContext.Clientes.FirstOrDefault(c => c.IdClientes == 1);

            Assert.NotNull(clienteExistente);

            clienteExistente.Nombres = "Fany Modificada";
            clienteExistente.Direccion = "Rioja";
            clienteExistente.Email = "fanymodificada@correo.com";
            _dbContext.Clientes.Update(clienteExistente);
            _dbContext.SaveChanges();

            var clienteActualizado = _dbContext.Clientes.FirstOrDefault(c => c.IdClientes == 1);

            Assert.NotNull(clienteActualizado);
            Assert.Equal("Fany Modificada", clienteActualizado.Nombres);
            Assert.Equal("Rioja", clienteActualizado.Direccion);
            Assert.Equal("fanymodificada@correo.com", clienteActualizado.Email);
        }
        [Fact]
        public async Task ValidarCampos_ClientesMayoristas()
        {
            var clienteInvalido = new Clientes
            {
                IdClientes = 3,
                Apellidos = "Gonzales",
                Dni = "12345678",
                Celular = "987654321",
                Direccion = "Tarapoto",
                FechaRegistro = DateTime.Now,
                Descripcion = "Mayorista",
                Estado = true,
                Email = "" 
                           
            };
            if (string.IsNullOrWhiteSpace(clienteInvalido.Nombres))
                _adminController.ModelState.AddModelError("Nombres", "El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(clienteInvalido.Email))
                _adminController.ModelState.AddModelError("Email", "El correo es obligatorio.");

            var resultado = await _adminController.Registrar_Cliente(clienteInvalido);

            var vista = Assert.IsType<ViewResult>(resultado);
            Assert.False(_adminController.ModelState.IsValid);
            Assert.True(_adminController.ModelState.ContainsKey("Nombres"));
            Assert.True(_adminController.ModelState.ContainsKey("Email"));
        }

        [Fact]
        public void EliminarCliente_Mayorista()
        {
            var cliente = _dbContext.Clientes.FirstOrDefault(c => c.IdClientes == 2);
            Assert.NotNull(cliente);
            _dbContext.Clientes.Remove(cliente);
            _dbContext.SaveChanges();
            var clienteEliminado = _dbContext.Clientes.FirstOrDefault(c => c.IdClientes == 2);
            Assert.Null(clienteEliminado);
        }

    }
}
