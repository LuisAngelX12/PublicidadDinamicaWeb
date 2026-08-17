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
    public class AnunciosController : AdminBaseController
    {
        private readonly AppDbContext _context;

        public AnunciosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Anuncios
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Anuncios.Include(a => a.Producto);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Anuncios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anuncio = await _context.Anuncios
                .Include(a => a.Producto)
                .FirstOrDefaultAsync(m => m.IdAnuncio == id);
            if (anuncio == null)
            {
                return NotFound();
            }

            return View(anuncio);
        }

        // GET: Anuncios/Create
        public IActionResult Create()
        {
            ViewData["IdProducto"] = new SelectList(_context.Productos, "IdProducto", "NombreProducto");
            return View();
        }

        // POST: Anuncios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdAnuncio,IdProducto,Titulo,TextoPromocional,FechaInicio,FechaFin,Estado")] Anuncio anuncio)
        {
            if (!ModelState.IsValid)
            {
                ConvertirFechasUtc(anuncio);

                _context.Add(anuncio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdProducto"] = new SelectList(_context.Productos, "IdProducto", "NombreProducto", anuncio.IdProducto);
            return View(anuncio);
        }

        // GET: Anuncios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anuncio = await _context.Anuncios.FindAsync(id);
            if (anuncio == null)
            {
                return NotFound();
            }
            ViewData["IdProducto"] = new SelectList(_context.Productos, "IdProducto", "NombreProducto", anuncio.IdProducto);
            return View(anuncio);
        }

        // POST: Anuncios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAnuncio,IdProducto,Titulo,TextoPromocional,FechaInicio,FechaFin,Estado")] Anuncio anuncio)
        {
            if (id != anuncio.IdAnuncio)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    ConvertirFechasUtc(anuncio);

                    _context.Update(anuncio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnuncioExists(anuncio.IdAnuncio))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdProducto"] = new SelectList(_context.Productos, "IdProducto", "NombreProducto", anuncio.IdProducto);
            return View(anuncio);
        }

        // GET: Anuncios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var anuncio = await _context.Anuncios
                .Include(a => a.Producto)
                .FirstOrDefaultAsync(m => m.IdAnuncio == id);
            if (anuncio == null)
            {
                return NotFound();
            }

            return View(anuncio);
        }

        // POST: Anuncios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var anuncio = await _context.Anuncios.FindAsync(id);
            if (anuncio != null)
            {
                _context.Anuncios.Remove(anuncio);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private void ConvertirFechasUtc(Anuncio anuncio)
        {
            if (anuncio.FechaInicio.HasValue)
            {
                anuncio.FechaInicio = DateTime.SpecifyKind(
                    anuncio.FechaInicio.Value,
                    DateTimeKind.Utc);
            }

            if (anuncio.FechaFin.HasValue)
            {
                anuncio.FechaFin = DateTime.SpecifyKind(
                    anuncio.FechaFin.Value,
                    DateTimeKind.Utc);
            }
        }

        private bool AnuncioExists(int id)
        {
            return _context.Anuncios.Any(e => e.IdAnuncio == id);
        }
    }
}
