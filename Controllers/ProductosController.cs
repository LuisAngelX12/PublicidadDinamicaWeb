namespace PublicidadDinamicaWeb.Controllers
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using PublicidadDinamicaWeb.Data;
    using PublicidadDinamicaWeb.Models;
    using PublicidadDinamicaWeb.Models.PublicidadDinamicaWeb.Models;
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using static PublicidadDinamicaWeb.Models.StaticData;

    public class ProductosController : AdminBaseController
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductosController(
            AppDbContext context,
            IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (HttpContext.Session.GetInt32("IdUsuario") == null)
            {
                context.Result = RedirectToAction("Login", "Account");
            }
            base.OnActionExecuting(context);
        }

        // GET: Productos
        public async Task<IActionResult> Index()
        {
            var productos = await _context.Productos
                .Include(p => p.HistorialPrecios.OrderBy(h => h.FechaCambio))
                .ToListAsync();
    
            return View(productos);
        }


        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Productos
                .Include(p => p.Comercio)
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.IdProducto == id);

            if (producto == null) return NotFound();

            return View(producto);
        }

        // GET: Productos/Create
        public IActionResult Create()
        {
            CargarCombos();
            return View();
        }

        // POST: Productos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Producto producto, IFormFile imagenFile)
        {
            if (ModelState.IsValid)
            {
                CargarCombos(producto);
                return View(producto);
            }

            if (producto.PrecioActual < 0 || producto.PrecioActual > 99999999.99m)
            {
                ModelState.AddModelError("PrecioActual", "El precio está fuera del rango permitido.");
                CargarCombos(producto);
                return View(producto);
            }

            if (imagenFile != null && imagenFile.Length > 0)
            {
                string carpeta = Path.Combine(_env.WebRootPath, "images/productos");
                Directory.CreateDirectory(carpeta);

                string nombreArchivo = Guid.NewGuid() + Path.GetExtension(imagenFile.FileName);
                string rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                using var stream = new FileStream(rutaCompleta, FileMode.Create);
                await imagenFile.CopyToAsync(stream);

                producto.Imagen = nombreArchivo;
            }

            // Manejar la versión de pantalla
            var version = await _context.VersionPantalla.FirstOrDefaultAsync();

            if (version == null)
            {
                version = new VersionPantalla { Valor = 1 };
                _context.VersionPantalla.Add(version);
            }
            else
            {
                version.Valor++;
            }

            _context.Add(producto);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound();

            CargarCombos(producto);
            return View(producto);
        }

        // POST: Productos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Producto producto, IFormFile imagenFile, bool EliminarImagen)
        {

            if (producto.PrecioActual < 0 || producto.PrecioActual > 99999999.99m)
            {
                ModelState.AddModelError("PrecioActual", "El precio está fuera del rango permitido.");
                CargarCombos(producto);
                return View(producto);
            }

            var productoDb = await _context.Productos.FindAsync(producto.IdProducto);
            if (productoDb == null) return NotFound();

            // 🔹 GUARDAR PRECIO ANTERIOR
            var precioAnterior = productoDb.PrecioActual;

            // 🔹 ACTUALIZAR CAMPOS
            productoDb.NombreProducto = producto.NombreProducto;
            productoDb.Descripcion = producto.Descripcion;
            productoDb.PrecioActual = producto.PrecioActual;
            productoDb.IdCategoria = producto.IdCategoria;
            productoDb.IdComercio = producto.IdComercio;
            productoDb.Estado = producto.Estado;

            // 🔹 HISTORIAL DE PRECIOS
            if (precioAnterior != producto.PrecioActual)
            {
                _context.HistorialPrecios.Add(new HistorialPrecio
                {
                    IdProducto = productoDb.IdProducto,
                    Producto = productoDb,
                    PrecioAnterior = precioAnterior,
                    PrecioNuevo = producto.PrecioActual,
                    FechaCambio = DateTime.UtcNow
                });
            }

            // 🔹 HISTORIAL DE PRECIOS
            if (precioAnterior != producto.PrecioActual)
            {
                _context.HistorialPrecios.Add(new HistorialPrecio
                {
                    IdProducto = productoDb.IdProducto,
                    Producto = productoDb,
                    PrecioAnterior = precioAnterior,
                    PrecioNuevo = producto.PrecioActual,
                    FechaCambio = DateTime.UtcNow
                });
            }

            // 🔹 IMAGEN
            if (EliminarImagen)
            {
                if (!string.IsNullOrEmpty(productoDb.Imagen))
                {
                    var path = Path.Combine(_env.WebRootPath, "images/productos", productoDb.Imagen);
                    if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
                }
                productoDb.Imagen = null;
            }
            else if (imagenFile != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(imagenFile.FileName);
                var filePath = Path.Combine(_env.WebRootPath, "images/productos", fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await imagenFile.CopyToAsync(stream);
                productoDb.Imagen = fileName;
            }

            // Manejar la versión de pantalla
            var version = await _context.VersionPantalla.FirstOrDefaultAsync();

            if (version == null)
            {
                version = new VersionPantalla { Valor = 1 };
                _context.VersionPantalla.Add(version);
            }
            else
            {
                version.Valor++;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Productos
                .Include(p => p.Comercio)
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(m => m.IdProducto == id);

            if (producto == null) return NotFound();

            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto != null)
            {
                if (!string.IsNullOrEmpty(producto.Imagen))
                {
                    var path = Path.Combine(_env.WebRootPath, "images/productos", producto.Imagen);

                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                }

                _context.Productos.Remove(producto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // 🔹 Método seguro para combos
        private void CargarCombos(Producto? producto = null)
        {
            ViewBag.Comercios = new SelectList(
                _context.Comercios,
                "IdComercio",
                "NombreComercio",
                producto?.IdComercio
            );

            ViewBag.Categorias = new SelectList(
                _context.Categorias,
                "IdCategoria",
                "NombreCategoria",
                producto?.IdCategoria
            );
        }
    }
}
