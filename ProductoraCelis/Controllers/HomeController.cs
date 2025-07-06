    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using ProductoraCelis.Data;
    using ProductoraCelis.Models;
    using ProductoraCelis.ViewsModes;
using QuestPDF.Fluent;
using System.Diagnostics;
    using System.Security.Claims;
    using System.Threading.Tasks;



namespace ProductoraCelis.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly pgDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, pgDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Acceso");
        }

        public async Task<IActionResult> Productos()
        {
            var listaProductos = await _dbContext.Productos.ToListAsync();
            return View(listaProductos);
        }

        public IActionResult ResumenCarrito()
        {
            string usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var carrito = _dbContext.Carrito.Where(c => c.UsuarioId == usuarioId).ToList();
            var carritoJson = JsonConvert.SerializeObject(carrito);
            HttpContext.Session.SetString("Carrito", carritoJson);
            return View(carrito);
        }


        [HttpPost]
        public IActionResult LimpiarCarrito()
        {
            string usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var items = _dbContext.Carrito.Where(c => c.UsuarioId == usuarioId);
            _dbContext.Carrito.RemoveRange(items);
            _dbContext.SaveChanges();

            return RedirectToAction("ResumenCarrito");
        }

        public IActionResult AgregarAlCarrito(int id)
        {
            string usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var producto = _dbContext.Productos.FirstOrDefault(p => p.IdProducto == id);
            if (producto == null)
                return NotFound();

            var carritoItem = _dbContext.Carrito
                .FirstOrDefault(c => c.IdProducto == id && c.UsuarioId == usuarioId);

            if (carritoItem != null)
            {
                carritoItem.Cantidad++;
                _dbContext.Carrito.Update(carritoItem);
            }
            else
            {
                var nuevoItem = new Carrito
                {
                    UsuarioId = usuarioId,
                    IdProducto = producto.IdProducto,
                    Nombre = producto.Nombre,
                    Precio = producto.Precio,
                    Cantidad = 1
                };
                _dbContext.Carrito.Add(nuevoItem);
            }
            _dbContext.SaveChanges();
            return RedirectToAction("ResumenCarrito");


        }


        [HttpPost]
        public IActionResult AgregarVariosAlCarrito(AgregarProductosVM model)
        {
            string usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var productosSeleccionados = model.Items
             .Where(p => p.Cantidad > 0)
            .ToList();

            foreach (var item in productosSeleccionados)
            {
                var carrito = new Carrito
                {
                    UsuarioId = usuarioId,
                    IdProducto = item.IdProducto,
                    Nombre = item.Nombre,
                    Categoria = item.Categoria,
                    Precio = item.Precio,
                    Cantidad = item.Cantidad,
                    UrlImagen = item.UrlImagen
                };

                _dbContext.Carrito.Add(carrito);
            }
            _dbContext.SaveChanges();
            return RedirectToAction("ResumenCarrito");
        }
        [HttpPost]
        public IActionResult DisminuirCantidad(int idProducto)
        {
            string usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var carritoItem = _dbContext.Carrito
                .FirstOrDefault(c => c.IdProducto == idProducto && c.UsuarioId == usuarioId);

            if (carritoItem == null)
                return NotFound();

            if (carritoItem.Cantidad > 1)
            {
                carritoItem.Cantidad--;
                _dbContext.Carrito.Update(carritoItem);
            }
            else
            {
                // Si la cantidad es 1 y se disminuye, se elimina el item del carrito
                _dbContext.Carrito.Remove(carritoItem);
            }

            _dbContext.SaveChanges();
            return RedirectToAction("ResumenCarrito");
        }

        [HttpPost]
        public IActionResult AumentarCantidad(int idProducto)
        {
            string usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var carritoItem = _dbContext.Carrito
                .FirstOrDefault(c => c.IdProducto == idProducto && c.UsuarioId == usuarioId);

            if (carritoItem == null)
                return NotFound();

            carritoItem.Cantidad++;
            _dbContext.Carrito.Update(carritoItem);
            _dbContext.SaveChanges();

            return RedirectToAction("ResumenCarrito");
        }


        public IActionResult RealizarPago()
        {
            var carritoJson = HttpContext.Session.GetString("Carrito");
            var carrito = string.IsNullOrEmpty(carritoJson)
                ? new List<Carrito>()
                : JsonConvert.DeserializeObject<List<Carrito>>(carritoJson) ?? new List<Carrito>();

            if (!carrito.Any())
            {
                TempData["Mensaje"] = "Tu carrito está vacío.";
                return RedirectToAction("ResumenCarrito");
            }

            var model = new PagoVM
            {
                Carrito = carrito,
                Total = carrito.Sum(c => c.Precio * c.Cantidad)
            };

            return View("RealizarPago", model);
        }

        [HttpPost]
        public async Task<IActionResult> RealizarPago(PagoVM model)
        {
            var carritoJson = HttpContext.Session.GetString("Carrito");

            var carrito = string.IsNullOrEmpty(carritoJson)
                ? new List<Carrito>()
                : JsonConvert.DeserializeObject<List<Carrito>>(carritoJson) ?? new List<Carrito>();

            if (!carrito.Any())
            {
                TempData["Mensaje"] = "Tu carrito está vacío.";
                return RedirectToAction("ResumenCarrito");
            }

            // Validación si método pago es tarjeta
            if (model.MetodoPago == "Tarjeta")
            {
                if (string.IsNullOrEmpty(model.NumeroTarjeta) ||
                    string.IsNullOrEmpty(model.NombreTitular) ||
                    !model.FechaExpiracion.HasValue ||
                    string.IsNullOrEmpty(model.Cvv))
                {
                    ModelState.AddModelError("", "Todos los campos de tarjeta son obligatorios.");
                }
            }

            if (!ModelState.IsValid)
            {
                model.Carrito = carrito;
                model.Total = carrito.Sum(c => c.Precio * c.Cantidad);
                return View("RealizarPago", model);
            }

            // Obtener Id del usuario autenticado
            int userId = 0;
            if (User?.Identity?.IsAuthenticated==true)
            {
                // Suponiendo que el claim con Id de usuario se llama "Id"
                var userIdClaim = User.FindFirst("Id") ?? User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parsedUserId))
                {
                    userId = parsedUserId;
                }
                else
                {
                    // Manejar caso de claim no válido o no numérico
                    ModelState.AddModelError("", "No se pudo obtener el Id del usuario autenticado.");
                    model.Carrito = carrito;
                    model.Total = carrito.Sum(c => c.Precio * c.Cantidad);
                    return View(model);
                }
            }
            else
            {
                // Usuario no autenticado
                TempData["Mensaje"] = "Debes iniciar sesión para realizar el pago.";
                return RedirectToAction("Login", "Cuenta"); // Cambia según tu ruta de login
            }

            // Crear compra
            var compra = new Compra
            {
                FechaCompra = DateTime.Now,
                IdUsuarioCliente = userId,
                Total = carrito.Sum(c => c.Precio * c.Cantidad)
            };
            _dbContext.Compras.Add(compra);
            await _dbContext.SaveChangesAsync();

            // Guardar detalles compra
            foreach (var item in carrito)
            {
                var detalleCompra = new DetalleCompra
                {
                    IdDetalleCompra = compra.IdDetalleCompra,
                    IdCompra = compra.IdCompra,
                    IdProducto = item.IdProducto,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.Precio,
                    SubTotal = item.Precio * item.Cantidad
                };
                _dbContext.DetallesCompra.Add(detalleCompra);
            }
            await _dbContext.SaveChangesAsync();

            // Registrar pago
            var pago = new Pago
            {
                IdCompra = compra.IdCompra,
                Monto = compra.Total,
                FechaPago = DateTime.Now,
                MetodoPago = model.MetodoPago,
                NumeroTarjeta = model.MetodoPago == "Tarjeta" ? model.NumeroTarjeta : null,
                NombreTitular = model.MetodoPago == "Tarjeta" ? model.NombreTitular : null,
                FechaExpiracion = model.MetodoPago == "Tarjeta" ? model.FechaExpiracion : null,
                Cvv = model.MetodoPago == "Tarjeta" ? model.Cvv : null
            };
            _dbContext.Pagos.Add(pago);
            await _dbContext.SaveChangesAsync();

            // Limpiar carrito
            HttpContext.Session.Remove("Carrito");

            TempData["Mensaje"] = "Pago realizado correctamente.";

            // Redirigir a comprobante  
            return RedirectToAction("Comprobante", new { idCompra = compra.IdCompra });
        }

        public IActionResult Comprobante(int idCompra)
        {
            Compra? compra = _dbContext.Compras
                .Include(c => c.DetallesCompra!)
                .ThenInclude(d => d.Producto)
                .Include(c => c.Pago)
                .FirstOrDefault(c => c.IdCompra == idCompra);

            if (compra == null || compra.DetallesCompra == null)
                return NotFound();

            var detalles = compra.DetallesCompra;

            var model = new ComprobanteVM
            {
                IdCompra = compra.IdCompra,
                Carrito = detalles.Select(d => new Carrito
                {
                    IdProducto = d.IdProducto,
                    Nombre = d.Producto?.Nombre ?? "Producto sin nombre",
                    Precio = d.PrecioUnitario,
                    Cantidad = d.Cantidad,
                    Categoria = d.Producto?.Categoria ?? "Sin categoría",
                    UrlImagen = d.Producto?.UrlImagen
                }).ToList(),
                Total = detalles.Sum(d => d.PrecioUnitario * d.Cantidad),
                MetodoPago = compra.Pago?.MetodoPago ?? "No especificado",
                FechaGeneracion = compra.FechaCompra
            };

            return View(model);
        }



        [HttpPost]
        public IActionResult Comprobante(string CarritoJson, string MetodoPago, string NumeroTarjeta, string NombreTitular, string FechaExpiracion, string Cvv, decimal Total)
        {
            var carrito = JsonConvert.DeserializeObject<List<Carrito>>(CarritoJson);

            int idUsuario;
            try
            {
                idUsuario = ObtenerUsuarioActual();
            }
            catch (Exception)
            {
                // Usuario no autenticado, redirigir al login o mostrar mensaje
                return RedirectToAction("Login", "Acceso");
            }

            var compra = new Compra
            {
                FechaCompra = DateTime.Now,
                IdUsuarioCliente = idUsuario,
                DetallesCompra = carrito!.Select(c => new DetalleCompra
                {
                    IdProducto = c.IdProducto,
                    Cantidad = c.Cantidad,
                    PrecioUnitario = c.Precio
                }).ToList(),
                Pago = new Pago
                {
                    MetodoPago = MetodoPago,
                    NumeroTarjeta = MetodoPago == "Tarjeta" ? NumeroTarjeta : null,
                    NombreTitular = MetodoPago == "Tarjeta" ? NombreTitular : null,
                    FechaExpiracion = MetodoPago == "Tarjeta" ? FechaTimeParse(FechaExpiracion) : null,
                    Cvv = MetodoPago == "Tarjeta" ? Cvv : null
                }
            };

            _dbContext.Compras.Add(compra);
            _dbContext.SaveChanges();

            return RedirectToAction("Comprobante", new { idCompra = compra.IdCompra });
        }

        private int ObtenerUsuarioActual()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                throw new Exception("Usuario no autenticado");
            }
            return int.Parse(claim.Value);
        }



        private DateTime? FechaTimeParse(string fecha)
        {
            if (DateTime.TryParseExact(fecha + "-01", "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime result))
                return result;
            return null;
        }

        [HttpPost]
        public async Task<JsonResult> GuardarPDF([FromBody] ComprobantePDFInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.pdfBase64))
                return Json(new { mensaje = "Datos inválidos." });

            try
            {
                byte[] pdfBytes = Convert.FromBase64String(input.pdfBase64);

                var comprobante = new Comprobante
                {
                    IdCompra = input.idCompra,
                    ArchivoPdf = pdfBytes,
                    FechaGeneracion = DateTime.Now,
                    RutaArchivo = $"Comprobante_{input.idCompra}.pdf"
                };

                _dbContext.Comprobantes.Add(comprobante);
                await _dbContext.SaveChangesAsync();

                return Json(new { mensaje = "Comprobante guardado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { mensaje = "Error al guardar el comprobante: " + ex.Message });
            }
        }
        public IActionResult VerComprobante(int id)
        {
            var comprobante = _dbContext.Comprobantes.FirstOrDefault(c => c.IdComprobante == id);

            if (comprobante == null || string.IsNullOrEmpty(comprobante.RutaArchivo))
                return NotFound("Comprobante no encontrado.");

            // Construir ruta física
            var rutaFisica = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", comprobante.RutaArchivo.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

            if (!System.IO.File.Exists(rutaFisica))
                return NotFound("Archivo físico no encontrado.");

            var bytes = System.IO.File.ReadAllBytes(rutaFisica);
            return File(bytes, "application/pdf", $"Comprobante_{comprobante.IdCompra}.pdf");
        }
        /*
        public IActionResult ReporteCompras()
        {
            var reporte = _dbContext.Comprobantes
                .Include(c => c.Compra)
                .Include(c => c.Usuario)
                .Select(c => new ReporteCompraVM
                {
                    IdCompra = c.IdCompra,
                    Usuario = c.Usuario.Nombre,
                    Fecha = c.Fecha,
                    RutaPDF = c.RutaArchivo,
                    Total = c.Compra.Total
                }).ToList();

            return View(reporte);
        }
        */
        public byte[] GenerarPdfDelComprobante(Compra compra)
        {
            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);

                    page.Header().Text($"Comprobante de Compra #{compra.IdCompra}")
                        .FontSize(20).Bold();

                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Cliente: {compra.UsuarioCliente?.Nombres}");
                        col.Item().Text($"Fecha: {compra.FechaCompra:dd/MM/yyyy}");
                        col.Item().Text($"Total: S/. {compra.Total}");

                        col.Item().PaddingVertical(10).Text("Detalles de productos:");

                        foreach (var item in compra.DetallesCompra!)
                        {
                            col.Item().Text($"- {item.Producto.Nombre} x {item.Cantidad} = S/. {item.PrecioUnitario}");
                        }
                    });

                    page.Footer().AlignCenter().Text("Gracias por su compra.");
                });
            }).GeneratePdf();

            return pdfBytes;
        }

    }

}


public class ComprobantePDFInput
{
    public int idCompra { get; set; }
    public string? pdfBase64 { get; set; }
}


