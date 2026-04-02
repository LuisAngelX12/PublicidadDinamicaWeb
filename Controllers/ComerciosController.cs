using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PublicidadDinamicaWeb.Data;
using PublicidadDinamicaWeb.Models;

namespace PublicidadDinamicaWeb.Controllers
{
    public class ComerciosController : AdminBaseController
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ComerciosController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Comercios
        public async Task<IActionResult> Index()
        {
            return View(await _context.Comercios.ToListAsync());
        }

        // GET: Comercios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comercio = await _context.Comercios
                .FirstOrDefaultAsync(m => m.IdComercio == id);
            if (comercio == null)
            {
                return NotFound();
            }

            return View(comercio);
        }

        // GET: Comercios/Create
        public IActionResult Create()
        {
            var usuarios = _context.Usuarios
                           .Select(u => new { u.IdUsuario, u.Nombre })
                           .ToList();

            ViewBag.Usuarios = new SelectList(usuarios, "IdUsuario", "Nombre");

            return View();
        }

        // POST: Comercios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdComercio,IdUsuario,NombreComercio,Descripcion,Estado")] Comercio comercio,IFormFile imagenFile)
        {
            if (imagenFile != null && imagenFile.Length > 0)
            {
                string carpeta = Path.Combine(_env.WebRootPath, "images/comercios");
                Directory.CreateDirectory(carpeta);

                string nombreArchivo = Guid.NewGuid() + Path.GetExtension(imagenFile.FileName);
                string rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                using var stream = new FileStream(rutaCompleta, FileMode.Create);
                await imagenFile.CopyToAsync(stream);

                comercio.Logo = nombreArchivo;
            }

            _context.Add(comercio);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Comercios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comercio = await _context.Comercios.FindAsync(id);
            if (comercio == null)
            {
                return NotFound();
            }

            var usuarios = _context.Usuarios
                       .Select(u => new { u.IdUsuario, u.Nombre })
                       .ToList();
            ViewBag.Usuarios = new SelectList(usuarios, "IdUsuario", "Nombre", comercio.IdUsuario);

            return View(comercio);
        }

        // POST: Comercios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,[Bind("IdComercio,IdUsuario,NombreComercio,Descripcion,Estado")] Comercio comercio,IFormFile imagenFile,bool EliminarImagen)
        {
            if (id != comercio.IdComercio)
                return NotFound();

            var comercioDb = await _context.Comercios.FindAsync(id);
            if (comercioDb == null)
                return NotFound();

            comercioDb.NombreComercio = comercio.NombreComercio;
            comercioDb.Descripcion = comercio.Descripcion;
            comercioDb.IdUsuario = comercio.IdUsuario;
            comercioDb.Estado = comercio.Estado;

            // 🔹 Eliminar imagen
            if (EliminarImagen)
            {
                if (!string.IsNullOrEmpty(comercioDb.Logo))
                {
                    var path = Path.Combine(_env.WebRootPath, "images/comercios", comercioDb.Logo);
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                }
                comercioDb.Logo = null;
            }
            // 🔹 Nueva imagen
            else if (imagenFile != null && imagenFile.Length > 0)
            {
                string carpeta = Path.Combine(_env.WebRootPath, "images/comercios");
                Directory.CreateDirectory(carpeta);

                string nombreArchivo = Guid.NewGuid() + Path.GetExtension(imagenFile.FileName);
                string rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                using var stream = new FileStream(rutaCompleta, FileMode.Create);
                await imagenFile.CopyToAsync(stream);

                comercioDb.Logo = nombreArchivo;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Comercios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comercio = await _context.Comercios
                .FirstOrDefaultAsync(m => m.IdComercio == id);
            if (comercio == null)
            {
                return NotFound();
            }

            return View(comercio);
        }

        // POST: Comercios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comercio = await _context.Comercios
                .Include(c => c.Productos)
                .FirstOrDefaultAsync(c => c.IdComercio == id);

            if (comercio == null)
                return NotFound();

            foreach (var producto in comercio.Productos)
            {
                if (!string.IsNullOrEmpty(producto.Imagen))
                {
                    var pathProducto = Path.Combine(
                        _env.WebRootPath,
                        "images/productos",
                        producto.Imagen
                    );

                    if (System.IO.File.Exists(pathProducto))
                        System.IO.File.Delete(pathProducto);
                }
            }

            if (!string.IsNullOrEmpty(comercio.Logo))
            {
                var pathComercio = Path.Combine(
                    _env.WebRootPath,
                    "images/comercios",
                    comercio.Logo
                );

                if (System.IO.File.Exists(pathComercio))
                    System.IO.File.Delete(pathComercio);
            }

            _context.Comercios.Remove(comercio);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private void CargarUsuarios(Comercio? comercio = null)
        {
            var usuarios = _context.Usuarios
                .Select(u => new { u.IdUsuario, u.Nombre })
                .ToList();

            ViewBag.Usuarios = new SelectList(
                usuarios,
                "IdUsuario",
                "Nombre",
                comercio?.IdUsuario
            );
        }

        private bool ComercioExists(int id)
        {
            return _context.Comercios.Any(e => e.IdComercio == id);
        }
    }
}
