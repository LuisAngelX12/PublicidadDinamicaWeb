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
            var hoy = DateTime.Now;

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

            // SOLO filtramos si comercioId tiene valor
            if (comercioId.HasValue)
            {
                query = query.Where(p => p.IdComercio == comercioId.Value);
            }

            var productos = await query.ToListAsync();

            var config = await _context.ConfiguracionPublicidad
                .Where(c => comercioId == null || c.IdComercio == comercioId)
                .FirstOrDefaultAsync(c => c.Activo)
                ?? new ConfiguracionPublicidad();

            var vm = new PublicidadViewModel
            {
                Productos = productos,
                Configuracion = config
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
