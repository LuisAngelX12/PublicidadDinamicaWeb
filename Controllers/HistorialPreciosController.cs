namespace PublicidadDinamicaWeb.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using PublicidadDinamicaWeb.Data;

    public class HistorialPreciosController : AdminBaseController
    {
        private readonly AppDbContext _context;

        public HistorialPreciosController(AppDbContext context)
        {
            _context = context;
        }

        // Historial por producto
        public async Task<IActionResult> PorProducto(int? id)
        {
            var query = _context.HistorialPrecios
                                .Include(h => h.Producto)
                                .AsQueryable();

            if (id.HasValue)
            {
                query = query.Where(h => h.IdProducto == id.Value);
                ViewBag.NombreProducto = (await query.FirstOrDefaultAsync())?.Producto.NombreProducto;
            }
            else
            {
                ViewBag.NombreProducto = "Historial completo";
            }

            var historial = await query.OrderByDescending(h => h.FechaCambio).ToListAsync();

            return View(historial);
        }
    }
}