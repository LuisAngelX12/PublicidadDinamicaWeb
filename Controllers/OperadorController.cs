using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PublicidadDinamicaWeb.Data;
using PublicidadDinamicaWeb.Models;
using PublicidadDinamicaWeb.Models.PublicidadDinamicaWeb.Models;

namespace PublicidadDinamicaWeb.Controllers
{
    public class OperadorController : SessionBaseController
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public OperadorController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ===============================
        // PANEL PRINCIPAL DEL OPERADOR
        // ===============================
        public async Task<IActionResult> Index()
        {
            var comercio = await _context.Comercios
                .Include(c => c.Productos)
                .ThenInclude(p => p.Categoria)
                .FirstOrDefaultAsync(c => c.IdUsuario == UsuarioId);

            if (comercio == null)
            {
                return View("CrearPrimerComercio");
            }

            return View(comercio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearPrimerComercio(Comercio comercio, IFormFile? imagenFile)
        {
            if (ModelState.IsValid)
                return View("CrearPrimerComercio", comercio);

            comercio.IdUsuario = UsuarioId;
            comercio.Estado = true;

            string carpeta = Path.Combine(_env.WebRootPath, "images/comercios");
            Directory.CreateDirectory(carpeta);

            if (imagenFile != null && imagenFile.Length > 0)
            {
                var nombreArchivo = Guid.NewGuid() + Path.GetExtension(imagenFile.FileName);
                var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                using var stream = new FileStream(rutaCompleta, FileMode.Create);
                await imagenFile.CopyToAsync(stream);

                comercio.Logo = nombreArchivo;
            }

            _context.Comercios.Add(comercio);
            await _context.SaveChangesAsync();

            // ACTUALIZAR SESIÓN CON EL NUEVO COMERCIO
            HttpContext.Session.SetInt32("ComercioId", comercio.IdComercio);

            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // COMERCIO
        // ===============================

        // GET: Operador/EditarComercio
        public async Task<IActionResult> EditarComercio()
        {
            var comercio = await _context.Comercios
                .FirstOrDefaultAsync(c => c.IdUsuario == UsuarioId);

            if (comercio == null) return NotFound();

            return View(comercio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarComercio(Comercio comercio, IFormFile imagenFile, bool EliminarImagen)
        {
            var comercioDb = await _context.Comercios
                .FirstOrDefaultAsync(c => c.IdUsuario == UsuarioId);

            if (comercioDb == null) return NotFound();

            comercioDb.NombreComercio = comercio.NombreComercio;
            comercioDb.Descripcion = comercio.Descripcion;
            comercioDb.Estado = comercio.Estado;

            // Eliminar imagen
            if (EliminarImagen && !string.IsNullOrEmpty(comercioDb.Logo))
            {
                var path = Path.Combine(_env.WebRootPath, "images/comercios", comercioDb.Logo);
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                comercioDb.Logo = null;
            }

            // Subir nueva imagen
            if (imagenFile != null && imagenFile.Length > 0)
            {
                var carpeta = Path.Combine(_env.WebRootPath, "images/comercios");
                Directory.CreateDirectory(carpeta);

                var nombreArchivo = Guid.NewGuid() + Path.GetExtension(imagenFile.FileName);
                var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                using var stream = new FileStream(rutaCompleta, FileMode.Create);
                await imagenFile.CopyToAsync(stream);

                comercioDb.Logo = nombreArchivo;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // PRODUCTOS
        // ===============================

        // GET: Operador/CrearProducto
        public IActionResult CrearProducto()
        {
            CargarCombos();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearProducto(Producto producto, IFormFile imagenFile)
        {
            producto.IdComercio = await _context.Comercios
                .Where(c => c.IdUsuario == UsuarioId)
                .Select(c => c.IdComercio)
                .FirstOrDefaultAsync();

            if (imagenFile != null && imagenFile.Length > 0)
            {
                var carpeta = Path.Combine(_env.WebRootPath, "images/productos");
                Directory.CreateDirectory(carpeta);

                var nombreArchivo = Guid.NewGuid() + Path.GetExtension(imagenFile.FileName);
                var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                using var stream = new FileStream(rutaCompleta, FileMode.Create);
                await imagenFile.CopyToAsync(stream);

                producto.Imagen = nombreArchivo;
            }

            _context.Add(producto);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Operador/EditarProducto/5
        public async Task<IActionResult> EditarProducto(int? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Productos
                .FirstOrDefaultAsync(p => p.IdProducto == id && p.Comercio!.IdUsuario == UsuarioId);

            if (producto == null) return NotFound();

            CargarCombos(producto);
            return View(producto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarProducto(Producto producto, IFormFile imagenFile, bool EliminarImagen)
        {
            var productoDb = await _context.Productos
                .Include(p => p.Comercio)
                .FirstOrDefaultAsync(p => p.IdProducto == producto.IdProducto && p.Comercio!.IdUsuario == UsuarioId);

            if (productoDb == null) return NotFound();

            // Actualizar campos
            productoDb.NombreProducto = producto.NombreProducto;
            productoDb.Descripcion = producto.Descripcion;
            productoDb.PrecioActual = producto.PrecioActual;
            productoDb.IdCategoria = producto.IdCategoria;
            productoDb.Estado = producto.Estado;

            // Imagen
            if (EliminarImagen && !string.IsNullOrEmpty(productoDb.Imagen))
            {
                var path = Path.Combine(_env.WebRootPath, "images/productos", productoDb.Imagen);
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                productoDb.Imagen = null;
            }

            if (imagenFile != null && imagenFile.Length > 0)
            {
                var nombreArchivo = Guid.NewGuid() + Path.GetExtension(imagenFile.FileName);
                var rutaCompleta = Path.Combine(_env.WebRootPath, "images/productos", nombreArchivo);

                using var stream = new FileStream(rutaCompleta, FileMode.Create);
                await imagenFile.CopyToAsync(stream);

                productoDb.Imagen = nombreArchivo;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Operador/EliminarProducto/5
        public async Task<IActionResult> EliminarProducto(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.Comercio)
                .FirstOrDefaultAsync(p => p.IdProducto == id && p.Comercio!.IdUsuario == UsuarioId);

            if (producto == null) return NotFound();

            if (!string.IsNullOrEmpty(producto.Imagen))
            {
                var path = Path.Combine(_env.WebRootPath, "images/productos", producto.Imagen);

                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // MÉTODOS AUXILIARES
        // ===============================
        private void CargarCombos(Producto? producto = null)
        {
            ViewBag.Categorias = new SelectList(_context.Categorias, "IdCategoria", "NombreCategoria", producto?.IdCategoria);
        }
    }
}