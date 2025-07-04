using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ProductoraCelis.Controllers;
using ProductoraCelis.Data;
using ProductoraCelis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PruebasUnitarias_ProductoraCelis
{
    public class Registrar_Proveedores
    {
        private readonly AdminController _adminController;
        private readonly HomeController _homeController;
        private readonly pgDbContext _dbContext;
        public Registrar_Proveedores()
        {
            var options = new DbContextOptionsBuilder<pgDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new pgDbContext(options);

            var mockLogger = new Mock<ILogger<AdminController>>();

            _adminController = new AdminController(_dbContext);

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "1") };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            _adminController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var proveedores = new List<Proveedor>
            {
                new Proveedor { IdProveedor = 1, Nombres = "Fany 1", Apellidos = "Palomino Yupanqui", Dni = "45632145", Celular = "987456312", Direccion= "Nuevo Cajamarca", Descripcion="Cliente de leche", Tipo= "Mayorista", Estado= true, Email="fani1235@hmail.com", FechaRegistro= DateTime.Now },
                new Proveedor { IdProveedor = 2, Nombres = "Fany 2", Apellidos = "Palomino Yupanqui 2", Dni = "4563215", Celular = "987456302", Direccion= "Cajamarca", Descripcion="Cliente de Quesos", Tipo= "Mayorista", Estado= true, Email="fany@hmail.com", FechaRegistro= DateTime.Now }
            };
            _dbContext.Proveedor.AddRange(proveedores);
            _dbContext.SaveChanges();
        }
        [Fact]
        public void EditarProveedor()
        {
            var proveedor = _dbContext.Proveedor.First(p => p.IdProveedor == 1);
            proveedor.Nombres = "Modificado";
            _dbContext.Proveedor.Update(proveedor);
            _dbContext.SaveChanges();

            var proveedorActualizado = _dbContext.Proveedor.First(p => p.IdProveedor == 1);
            Assert.Equal("Modificado", proveedorActualizado.Nombres);
        }
        [Fact]
        public async Task ValidarCampos_Proveedor()
        {
            var proveedorInvalido = new Proveedor
            {
                Apellidos = "Lopez", Dni = "12345678", Celular = "987654321",Direccion = "Lima", FechaRegistro = DateTime.Now,Descripcion = "Proveedor inválido",Tipo = "Minorista",Estado = true, Email = "invalido@email.com"
            };

            _adminController.ModelState.AddModelError("Nombres", "El campo Nombres es obligatorio.");

            var resultado = await _adminController.Registrar_Proveedor(proveedorInvalido);
            var vista = Assert.IsType<ViewResult>(resultado);
            var modeloDevuelto = Assert.IsType<Proveedor>(vista.Model);
            Assert.Equal("Lopez", modeloDevuelto.Apellidos);
            var proveedorGuardado = await _dbContext.Proveedor.FirstOrDefaultAsync(p => p.Email == "invalido@email.com");
            Assert.Null(proveedorGuardado);
        }


        [Fact]
        public void EliminarProveedor()
        {
            var proveedor = _dbContext.Proveedor.First(p => p.IdProveedor == 2);

            _dbContext.Proveedor.Remove(proveedor);
            _dbContext.SaveChanges();

            var proveedorEliminado = _dbContext.Proveedor.FirstOrDefault(p => p.IdProveedor == 2);
            Assert.Null(proveedorEliminado);
        }

    }
}
