using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductoraCelis.Data;
using ProductoraCelis.ViewsModes;
using ProductoraCelis.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

[Authorize(Roles = "Administrador")]
public class AdminController : Controller
{
    private readonly pgDbContext _dbContext;

    public AdminController(pgDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // Vista del panel de control del administrador

    public IActionResult Dashboard()
    {
        var viewModel = new AdminVM
        {
            TotalClientes = _dbContext.Clientes.Count(),
            TotalProveedores = _dbContext.Proveedor.Count(),
            TotalProductos = _dbContext.Productos.Count(),

            Proveedores = _dbContext.Proveedor.ToList(),
            Productos= _dbContext.Productos.ToList(),
            Clientes = _dbContext.Clientes.ToList(),

        };

        return View(viewModel);
        
    }
    // Método cliente
    [HttpGet]
    public async Task<IActionResult> Lista_Clientes()
    {
        List<Clientes> listaClientes = await _dbContext.Clientes.ToListAsync();
        return View(listaClientes);
    }
    [HttpGet]
    public IActionResult Registrar_Cliente()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Registrar_Cliente(Clientes cliente)
    {
        if (!ModelState.IsValid)
        {
            return View(cliente);
        }
        cliente.FechaRegistro = DateTime.Now;
        await _dbContext.Clientes.AddAsync(cliente);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Dashboard));
    }
    [HttpGet]
    public async Task<IActionResult> Editar_Cliente(int id)
    {
        Clientes clientes =await _dbContext.Clientes.FirstAsync(c=>c.IdClientes==id);
        return View(clientes);
    }
    [HttpPost]
    public async Task<IActionResult>Editar_Cliente(Clientes cliente)
    {
        _dbContext.Clientes.Update(cliente); 
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Dashboard));
    }
    [HttpGet]
    public async Task<IActionResult>Eliminar_Clientes(int id)
    {
        Clientes clientes=await _dbContext.Clientes.FirstAsync(c=>c.IdClientes==id);
        _dbContext.Clientes.Remove(clientes);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Dashboard));
    }


    // Método proveedor
    [HttpGet]
    public async Task<IActionResult> Lista_Proveedores()
    {
        List<Proveedor> listaProveedores = await _dbContext.Proveedor.ToListAsync();
        return View(listaProveedores);
    }
    [HttpGet]
    public IActionResult Registrar_Proveedor()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Registrar_Proveedor(Proveedor proveedor)
    {
        if (!ModelState.IsValid)
        {
            return View(proveedor);
        }
        proveedor.FechaRegistro = DateTime.Now;
        await _dbContext.Proveedor.AddAsync(proveedor);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Dashboard));

    }
    [HttpGet]
    public async Task<IActionResult> Editar_Proveedor(int id)
    {
        Proveedor proveedor = await _dbContext.Proveedor.FirstAsync(p => p.IdProveedor == id);
        return View(proveedor);

    }
    [HttpPost]
    public async Task<IActionResult> Editar_Proveedor(Proveedor proveedor)
    {
        _dbContext.Proveedor.Update(proveedor); 
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Dashboard));
    }
    [HttpGet]
    public async Task<IActionResult> Eliminar_Proveedor(int id)
    {
        Proveedor proveedor= await _dbContext.Proveedor.FirstAsync(p => p.IdProveedor == id);
        _dbContext.Proveedor.Remove(proveedor);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Dashboard));
    }

    // Método producto
    [HttpGet]
    public async Task<IActionResult> Lista_Productos()
    {
        List<Producto> listaProductos = await _dbContext.Productos.ToListAsync();
        return View(listaProductos);
    }
    [HttpGet]
    public IActionResult Registrar_Producto()
    {
       return View();
    }

    [HttpPost]
    public async Task<IActionResult> Registrar_Producto(Producto producto, IFormFile Imagen)
    {
        if (Imagen != null && Imagen.Length > 0)
        {
            var fileName = Path.GetFileNameWithoutExtension(Imagen.FileName);
            var extension = Path.GetExtension(Imagen.FileName);
            var uniqueFileName = $"{fileName}_{Guid.NewGuid()}{extension}";
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            var filePath = Path.Combine(uploadPath, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await Imagen.CopyToAsync(stream);
            }
            producto.UrlImagen = "/images/" + uniqueFileName;           
            await _dbContext.Productos.AddAsync(producto);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Dashboard));
            
        }
        return View(producto);
    }
    [HttpGet]
    public async Task<IActionResult> Editar_Producto(int id)
    {
        Producto producto =await _dbContext.Productos.FirstAsync(p =>p.IdProducto == id);
        return View(producto);
    }
    /*
    [HttpPost]
    public async Task<IActionResult> Editar_Producto(Producto producto, IFormFile Imagen)
    {
        if (Imagen != null && Imagen.Length > 0)
        {
            var fileName = Path.GetFileNameWithoutExtension(Imagen.FileName);
            var extension = Path.GetExtension(Imagen.FileName);
            var uniqueFileName = $"{fileName}_{Guid.NewGuid()}{extension}";
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            var filePath = Path.Combine(uploadPath, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await Imagen.CopyToAsync(stream);
            }
            producto.UrlImagen = "/images/" + uniqueFileName;
            _dbContext.Productos.Update(producto);
            await _dbContext.SaveChangesAsync();

        }
        return RedirectToAction(nameof(Dashboard));
    }*/
    [HttpPost]
    public async Task<IActionResult> Editar_Producto(Producto producto, IFormFile Imagen)
    {
        var productoExistente = await _dbContext.Productos.FindAsync(producto.IdProducto);
        if (productoExistente == null)
        {
            return NotFound();
        }

        // Actualiza propiedades
        productoExistente.Nombre = producto.Nombre;
        productoExistente.Descripcion = producto.Descripcion;
        productoExistente.Categoria = producto.Categoria;
        productoExistente.Precio = producto.Precio;
        if (Imagen != null && Imagen.Length > 0)
        {
            var fileName = Path.GetFileNameWithoutExtension(Imagen.FileName);
            var extension = Path.GetExtension(Imagen.FileName);
            var uniqueFileName = $"{fileName}_{Guid.NewGuid()}{extension}";
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }
            var filePath = Path.Combine(uploadPath, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await Imagen.CopyToAsync(stream);
            }
            productoExistente.UrlImagen = "/images/" + uniqueFileName;
        }

        await _dbContext.SaveChangesAsync();
        TempData["Mensaje"] = "Producto Actualizado";
        return RedirectToAction(nameof(Dashboard));
    }





    [HttpGet]
    public async Task<IActionResult> Eliminar_Producto(int id)
    {
        Producto producto = await _dbContext.Productos.FirstAsync(p => p.IdProducto == id);
        _dbContext.Productos.Remove(producto);
        await _dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Dashboard));
    }
    public async Task<List<Reporte>> GenerarReporteDesdeCarritoAsync(DateTime? fechaInicio = null, DateTime? fechaFin = null)
    {
        var query = _dbContext.Carrito.AsQueryable();

        if (fechaInicio.HasValue)
            query = query.Where(c => c.FechaHora >= fechaInicio.Value);

        if (fechaFin.HasValue)
            query = query.Where(c => c.FechaHora <= fechaFin.Value);

        var carritos = await query.ToListAsync();

        var reportes = carritos.Select(c => new Reporte
        {
            IdProducto = c.IdProducto,
            Categoria = c.Categoria,
            Cantidad = c.Cantidad,
            PrecioUnitario = c.Precio,
            Subtotal = c.Precio * c.Cantidad,
            TotalCantidadVendida = c.Cantidad,
            FechaHora = c.FechaHora
        }).ToList();

        return reportes;
    }

    [HttpGet]
    public async Task<IActionResult> ReporteCarrito(DateTime? fechaInicio, DateTime? fechaFin)
    {
        var reportes = await GenerarReporteDesdeCarritoAsync(fechaInicio, fechaFin);

        var vm = new ReporteCarritoVM
        {
            Reportes = reportes,
            FechaInicio = fechaInicio,
            FechaFin = fechaFin
        };

        return View(vm);
    }


}
