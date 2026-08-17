namespace PublicidadDinamicaWeb.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using PublicidadDinamicaWeb.Data;
    using PublicidadDinamicaWeb.Models;
    using static PublicidadDinamicaWeb.Models.StaticData;

    public class PublicidadController : Controller
    {
        private readonly AppDbContext _context;

        public PublicidadController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Pantalla(int? comercioId)
        {
            var hoy = DateTime.UtcNow;

            var query = _context.Productos
                .Where(p => p.Estado)
                .Include(p => p.Comercio)
                .Include(p => p.Categoria)
                .Include(p => p.HistorialPrecios)
                .Include(p => p.Anuncios.Where(a =>
                    a.Estado &&
                    (a.FechaInicio == null || a.FechaInicio <= hoy) &&
                    (a.FechaFin == null || a.FechaFin >= hoy)
                ))
                .AsQueryable();

            // Si se especifica un comercio, solo mostramos sus productos.
            // Si es null, mostramos todos los productos activos.
            if (comercioId.HasValue)
            {
                query = query.Where(p => p.IdComercio == comercioId.Value);
            }

            var productos = await query.ToListAsync();

            ConfiguracionPublicidad? config;

            if (comercioId.HasValue)
            {
                // Comercio específico:
                // busca SOLO su propia configuración activa.
                config = await _context.ConfiguracionPublicidad
                    .FirstOrDefaultAsync(c =>
                        c.IdComercio == comercioId.Value &&
                        c.Activo);
            }
            else
            {
                // Pantalla general:
                // busca SOLO la configuración del Admin (comercio 1).
                // Si Admin la desactiva, config será null.
                config = await _context.ConfiguracionPublicidad
                    .FirstOrDefaultAsync(c =>
                        c.IdComercio == 1 &&
                        c.Activo);
            }

            var vm = new PublicidadViewModel
            {
                Productos = productos,
                Configuracion = config ?? new ConfiguracionPublicidad()
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> VersionPantalla()
        {
            var version = await _context.VersionPantalla.FirstAsync();
            return Content(version.Valor.ToString());
        }
    }
}
