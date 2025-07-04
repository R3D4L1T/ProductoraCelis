using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductoraCelis.Controllers;
using ProductoraCelis.Data;
using ProductoraCelis.ViewsModes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebasUnitarias_ProductoraCelis
{
    public class Registro_CuentaCliente
    {
        [Fact]
        public async Task CuentaCliente()
        {
            var options = new DbContextOptionsBuilder<pgDbContext>()
                .UseInMemoryDatabase(databaseName: "RegistrarUsuarioDB")
                .Options;

            using var context = new pgDbContext(options);

            var controller = new AccesoController(context);

            var nuevoUsuario = new UsuarioVM
            {
                Nombres = "Ana",
                Apellidos = "Gomez",
                Celular = "987654321",
                Dni = "12345678",
                Email = "ana@example.com",
                Password = "password123",
                PasswordConfirm = "password123"
            };

            var result = await controller.Registrarse(nuevoUsuario);
            var usuarioEnDb = await context.Usuario.FirstOrDefaultAsync(u => u.Email == "ana@example.com");

            Assert.NotNull(usuarioEnDb);
            Assert.Equal("Ana", usuarioEnDb.Nombres);
            Assert.Equal("Cliente", usuarioEnDb.Rol);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectResult.ActionName);
            Assert.Equal("Acceso", redirectResult.ControllerName);
        }
        [Fact]
        public async Task ValidacionDe_Contraseñas()
        {
            var options = new DbContextOptionsBuilder<pgDbContext>()
                .UseInMemoryDatabase(databaseName: "RegistrarUsuarioDB2")
                .Options;

            using var context = new pgDbContext(options);
            var controller = new AccesoController(context);

            var nuevoUsuario = new UsuarioVM
            {
                Nombres = "Luis",
                Apellidos = "Perez",
                Celular = "987654321",
                Dni = "87654321",
                Email = "luis@example.com",
                Password = "password123",
                PasswordConfirm = "password321"  
            };

            var result = await controller.Registrarse(nuevoUsuario);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Las contraseñas no coinciden", controller.ViewData["Mensaje"]);

            var usuarioEnDb = await context.Usuario.FirstOrDefaultAsync(u => u.Email == "luis@example.com");
            Assert.Null(usuarioEnDb);
        }

    }
}
