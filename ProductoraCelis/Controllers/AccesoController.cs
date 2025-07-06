using ProductoraCelis.Data;
using ProductoraCelis.ViewsModes;
using ProductoraCelis.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ProductoraCelis.Controllers
{
    public class AccesoController : Controller
    {
        private readonly pgDbContext _pgDbContext;

        public AccesoController(pgDbContext pgDbContext)
        {
            _pgDbContext = pgDbContext;
        }

        [HttpGet]
        public IActionResult Registrarse()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrarse(UsuarioVM model)
        {
            if (model.Password != model.PasswordConfirm)
            {
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View();
            }

            Usuario usuario = new Usuario()
            {
                Nombres = model.Nombres,
                Apellidos = model.Apellidos,
                Celular = model.Celular,
                Dni = model.Dni,
                Email = model.Email,
                Password = model.Password,
                Rol = "Cliente"
            };

            await _pgDbContext.AddAsync(usuario);
            await _pgDbContext.SaveChangesAsync();

            if (usuario.IdUsuario != 0) return RedirectToAction("Login", "Acceso");

            ViewData["Mensaje"] = "No se pudo crear el usuario";
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM modelo)
        {
            // Buscar usuario en la base de datos
            Usuario? usuario_encontrado = await _pgDbContext.Usuario
                .Where(u => u.Email == modelo.Email && u.Password == modelo.Password)
                .FirstOrDefaultAsync();

            if (usuario_encontrado == null)
            {
                ViewData["Mensaje"] = "No se encontraron usuarios";
                return View();
            }

            // Crear claims para el usuario y añadir el rol
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, usuario_encontrado.Nombres),
                new Claim(ClaimTypes.NameIdentifier, usuario_encontrado.IdUsuario.ToString()),  // Agregar el NameIdentifier
                new Claim(ClaimTypes.Role, usuario_encontrado.Rol)  // Añadir rol a los claims
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties() { AllowRefresh = true };

            // Realizar la autenticación del usuario
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);

            // Redirigir según el rol del usuario
            if (usuario_encontrado.Rol == "Administrador")
            {
                return RedirectToAction("Dashboard","Admin");
            }
            else
            {
                return RedirectToAction("Index", "Home"); 
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Acceso");
        }

    }
}
