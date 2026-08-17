using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PublicidadDinamicaWeb.Data;
using PublicidadDinamicaWeb.Models;

namespace PublicidadDinamicaWeb.Controllers
{
    public class ClienteController : Controller
    {
        private readonly AppDbContext _context;

        public ClienteController(AppDbContext context)
        {
            _context = context;
        }

        // Vista principal del cliente: lista de comercios activos
        public async Task<IActionResult> Index()
        {
            var comercios = await _context.Comercios
                .Include(c => c.Productos.Where(p => p.Estado))
                .Where(c => c.Estado)
                .ToListAsync();

            // Creamos rutas de logos para cada comercio
            foreach (var comercio in comercios)
            {
                comercio.Logo = comercio.Logo != null
                    ? $"/images/comercios/{comercio.Logo}"
                    : "/images/comercios/placeholder.jpg";
            }

            return View(comercios);
        }

        // Vista de detalles de un comercio con sus productos
        public async Task<IActionResult> DetallesComercio(int id)
        {
            var comercio = await _context.Comercios
                .Include(c => c.Productos.Where(p => p.Estado))
                    .ThenInclude(p => p.Categoria)
                .FirstOrDefaultAsync(c => c.IdComercio == id && c.Estado);

            if (comercio == null)
                return NotFound();

            // Rutas de imágenes de productos
            foreach (var producto in comercio.Productos)
            {
                producto.Imagen = producto.Imagen != null
                    ? $"/images/productos/{producto.Imagen}"
                    : "/images/productos/placeholder.jpg";
            }

            // Logo del comercio
            comercio.Logo = comercio.Logo != null
                ? $"/images/comercios/{comercio.Logo}"
                : "/images/comercios/placeholder.jpg";

            return View(comercio);
        }

        // Vista de detalles de un producto
        public async Task<IActionResult> DetallesProducto(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.Comercio)
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.IdProducto == id && p.Estado);

            if (producto == null)
                return NotFound();

            // Ruta completa de la imagen del producto
            ViewBag.ImagenProducto = producto.Imagen != null
                ? $"/images/productos/{producto.Imagen}"
                : "/images/productos/placeholder.jpg";

            // Ruta completa del logo del comercio
            ViewBag.LogoComercio = producto.Comercio!.Logo != null
                ? $"/images/comercios/{producto.Comercio.Logo}"
                : "/images/comercios/placeholder.jpg";

            return View(producto);
        }
    }
}