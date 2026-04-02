using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PublicidadDinamicaWeb.Data;
using PublicidadDinamicaWeb.Models;
using PublicidadDinamicaWeb.Models.PublicidadDinamicaWeb.Models;

namespace PublicidadDinamicaWeb.Controllers
{
    public class ConfiguracionController : Controller
    {
        private readonly AppDbContext _context;

        public ConfiguracionController(AppDbContext context)
        {
            _context = context;
        }

        // ===============================
        // GET: Configuracion
        // ===============================
        public async Task<IActionResult> Index()
        {
            var comercioId = HttpContext.Session.GetInt32("ComercioId");

            if (comercioId == null || comercioId == 0)
                return RedirectToAction("Index", "Home");

            var config = await _context.ConfiguracionPublicidad
                .FirstOrDefaultAsync(c => c.IdComercio == comercioId);

            if (config == null)
            {
                config = new ConfiguracionPublicidad
                {
                    IdComercio = comercioId.Value
                };
            }

            return View(config);
        }

        // ===============================
        // POST: Configuracion
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ConfiguracionPublicidad model, IFormFile imagenFondo)
        {
            var comercioId = HttpContext.Session.GetInt32("ComercioId");

            if (comercioId == null || comercioId == 0)
                return RedirectToAction("Index", "Home");

            model.IdComercio = comercioId.Value;

            // ============================
            // Manejo de Imagen
            // ============================
            if (imagenFondo != null && imagenFondo.Length > 0)
            {
                var folderPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/images/fondos"
                );

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var fileName = Guid.NewGuid().ToString() +
                               Path.GetExtension(imagenFondo.FileName);

                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imagenFondo.CopyToAsync(stream);
                }

                model.ImagenFondo = fileName;
            }
            else
            {
                var existente = await _context.ConfiguracionPublicidad
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.IdComercio == comercioId);

                if (existente != null)
                    model.ImagenFondo = existente.ImagenFondo;
            }

            // ============================
            // Insertar o actualizar
            // ============================
            var configuracionExistente = await _context.ConfiguracionPublicidad
                .FirstOrDefaultAsync(c => c.IdComercio == comercioId);

            if (configuracionExistente == null)
            {
                _context.ConfiguracionPublicidad.Add(model);
            }
            else
            {
                model.IdConfiguracion = configuracionExistente.IdConfiguracion;
                _context.Entry(configuracionExistente)
                        .CurrentValues
                        .SetValues(model);
            }

            // ============================
            // VersionPantalla
            // ============================
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
    }
}