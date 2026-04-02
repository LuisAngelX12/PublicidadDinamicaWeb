namespace PublicidadDinamicaWeb.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using PublicidadDinamicaWeb.Data;
    using PublicidadDinamicaWeb.Models;

    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<Usuario> _passwordHasher = new PasswordHasher<Usuario>();

        private const string DefaultRoleName = "Usuario";
        private const string AdminRoleName = "Admin";
        private const string PantallaRoleName = "Pantalla";

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // LOGIN
        // =========================
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string correo, string contrasena)
        {
            if (string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(contrasena))
            {
                ViewBag.Error = "Credenciales incompletas.";
                return View();
            }

            // Buscar usuario solo por correo
            var usuario = await _context.Usuarios
                .Include(u => u.UsuarioRoles!)
                    .ThenInclude(ur => ur.Rol)
                .SingleOrDefaultAsync(u => u.Correo == correo);

            if (usuario == null)
            {
                ViewBag.Error = "Credenciales incorrectas";
                return View();
            }

            // 🔐 Verificar contraseña hasheada
            var resultado = _passwordHasher.VerifyHashedPassword(
                usuario,
                usuario.Contrasena,
                contrasena
            );

            if (resultado == PasswordVerificationResult.Failed)
            {
                ViewBag.Error = "Credenciales incorrectas";
                return View();
            }

            // Obtener roles
            var rolesAsignados = usuario.UsuarioRoles?
                .Select(ur => ur.Rol?.NombreRol)
                .Where(n => !string.IsNullOrEmpty(n))
                .ToList() ?? new List<string?>();

            if (!rolesAsignados.Any())
            {
                var usuarioRolesFromDb = await _context.UsuarioRol
                    .Where(ur => ur.IdUsuario == usuario.IdUsuario)
                    .Include(ur => ur.Rol)
                    .ToListAsync();

                rolesAsignados = usuarioRolesFromDb
                    .Select(ur => ur.Rol?.NombreRol)
                    .Where(n => !string.IsNullOrEmpty(n))
                    .ToList();
            }

            if (!rolesAsignados.Any())
            {
                rolesAsignados.Add(DefaultRoleName);
            }

            // Buscar comercio activo
            var comercio = await _context.Comercios
                .Where(c => c.IdUsuario == usuario.IdUsuario && c.Estado)
                .FirstOrDefaultAsync();

            int comercioId = comercio?.IdComercio ?? 0;

            // Guardar en sesión
            HttpContext.Session.SetInt32("IdUsuario", usuario.IdUsuario);
            HttpContext.Session.SetString("Nombre", usuario.Nombre);
            HttpContext.Session.SetString("Rol", rolesAsignados.First() ?? DefaultRoleName);
            HttpContext.Session.SetString("Roles", string.Join(",", rolesAsignados));
            HttpContext.Session.SetInt32("Estado", usuario.Estado ? 1 : 0);
            HttpContext.Session.SetInt32("ComercioId", comercioId);

            return RedirectToAction("Index", "Home");
        }

        // =========================
        // LOGOUT
        // =========================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // =========================
        // REGISTER
        // =========================
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string nombre, string correo, string contrasena, string? rol = null)
        {
            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(correo) ||
                string.IsNullOrWhiteSpace(contrasena))
            {
                ViewBag.Error = "Todos los campos son obligatorios.";
                return View();
            }

            if (await _context.Usuarios.AnyAsync(u => u.Correo == correo))
            {
                ViewBag.Error = "El correo ya está en uso.";
                return View();
            }

            await EnsureDefaultRolesExistAsync();

            var roleNameToAssign = !string.IsNullOrWhiteSpace(rol) ? rol! : DefaultRoleName;

            var rolEntity = await _context.Roles
                .SingleOrDefaultAsync(r => r.NombreRol == roleNameToAssign)
                ?? await _context.Roles
                .SingleOrDefaultAsync(r => r.NombreRol == DefaultRoleName);

            if (rolEntity == null)
            {
                ViewBag.Error = "No se pudo asignar un rol al usuario.";
                return View();
            }

            var nuevoUsuario = new Usuario
            {
                Nombre = nombre,
                Correo = correo,
                Estado = true,
                Contrasena = _passwordHasher.HashPassword(null!, contrasena),
                FechaRegistro = DateTime.Now,
                UsuarioRoles = new List<UsuarioRol>()
            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            var usuarioRol = new UsuarioRol
            {
                IdUsuario = nuevoUsuario.IdUsuario,
                Usuario = nuevoUsuario,
                IdRol = rolEntity.IdRol,
                Rol = rolEntity
            };

            _context.UsuarioRol.Add(usuarioRol);
            await _context.SaveChangesAsync();

            // Iniciar sesión automáticamente
            HttpContext.Session.SetInt32("IdUsuario", nuevoUsuario.IdUsuario);
            HttpContext.Session.SetString("Nombre", nuevoUsuario.Nombre);
            HttpContext.Session.SetString("Rol", rolEntity.NombreRol);
            HttpContext.Session.SetString("Roles", rolEntity.NombreRol);
            HttpContext.Session.SetInt32("Estado", nuevoUsuario.Estado ? 1 : 0);

            return RedirectToAction("Index", "Home");
        }

        // =========================
        // HELPERS
        // =========================
        private async Task EnsureDefaultRolesExistAsync()
        {
            if (!await _context.Roles.AnyAsync())
            {
                var roles = new[]
                {
                    new Rol { NombreRol = AdminRoleName, UsuarioRoles = new List<UsuarioRol>() },
                    new Rol { NombreRol = DefaultRoleName, UsuarioRoles = new List<UsuarioRol>() },
                    new Rol { NombreRol = PantallaRoleName, UsuarioRoles = new List<UsuarioRol>() }
                };

                _context.Roles.AddRange(roles);
                await _context.SaveChangesAsync();
            }
            else
            {
                if (!await _context.Roles.AnyAsync(r => r.NombreRol == DefaultRoleName))
                {
                    _context.Roles.Add(
                        new Rol
                        {
                            NombreRol = DefaultRoleName,
                            UsuarioRoles = new List<UsuarioRol>()
                        });
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}