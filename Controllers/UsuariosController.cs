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
    public class UsuariosController : AdminBaseController
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.UsuarioRoles)
                .ThenInclude(ur => ur.Rol)
                .Where(u => !u.UsuarioRoles
                    .Any(ur => ur.Rol.NombreRol == "Admin"))
                .ToListAsync();

            return View(usuarios);
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.UsuarioRoles)
                .ThenInclude(ur => ur.Rol)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);


            if (usuario == null)
            {
                return NotFound();
            }

            // Buscar el comercio asociado a ese usuario
            var comercio = await _context.Comercios
                .FirstOrDefaultAsync(c => c.IdUsuario == usuario.IdUsuario);

            ViewBag.Comercio = comercio;

            return View(usuario);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdUsuario,Nombre,Correo,Contrasena,Estado")] Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var usuario = await _context.Usuarios
                .Include(u => u.UsuarioRoles)
                .ThenInclude(ur => ur.Rol)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);

            if (usuario == null)
                return NotFound();

            var esAdmin = usuario.UsuarioRoles
                .Any(ur => ur.Rol.NombreRol == "Admin");

            if (esAdmin)
            {
                return RedirectToAction(nameof(Index));
            }

            // Obtener roles excepto Admin
            var roles = await _context.Roles
                .Where(r => r.NombreRol != "Admin")
                .ToListAsync();

            var rolActual = usuario.UsuarioRoles.FirstOrDefault();

            ViewBag.Roles = new SelectList(
                roles,
                "IdRol",
                "NombreRol",
                rolActual?.IdRol
            );

            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, bool Estado, int IdRol)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.UsuarioRoles)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);

            if (usuario == null)
                return NotFound();

            usuario.Estado = Estado;

            var usuarioRol = usuario.UsuarioRoles.FirstOrDefault();

            if (usuarioRol != null)
            {
                usuarioRol.IdRol = IdRol;
                _context.Update(usuarioRol);
            }
            else
            {
                var rol = await _context.Roles.FindAsync(IdRol);

                if (rol == null)
                {
                    return BadRequest("Rol inválido.");
                }

                var nuevoUsuarioRol = new UsuarioRol
                {
                    IdUsuario = usuario.IdUsuario,
                    IdRol = IdRol,
                    Usuario = usuario,
                    Rol = rol
                };

                _context.UsuarioRol.Add(nuevoUsuarioRol);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.IdUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.UsuarioRoles)
                .ThenInclude(ur => ur.Rol)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);

            if (usuario == null)
                return NotFound();

            var esAdmin = usuario.UsuarioRoles
                .Any(ur => ur.Rol.NombreRol == "Admin");

            if (esAdmin)
            {
                return RedirectToAction(nameof(Index));
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == id);
        }
    }
}
