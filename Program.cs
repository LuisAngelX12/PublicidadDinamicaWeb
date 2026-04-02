using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using PublicidadDinamicaWeb.Data;
using PublicidadDinamicaWeb.Models;
using System.Globalization;
using DotNetEnv;

Env.Load(); // Cargar variables del .env

var builder = WebApplication.CreateBuilder(args);

// =========================
// VARIABLES DE ENTORNO
// =========================
var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbUser = Environment.GetEnvironmentVariable("DB_USER");
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

var adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL");
var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");

var pantallaEmail = Environment.GetEnvironmentVariable("PANTALLA_EMAIL");
var pantallaPassword = Environment.GetEnvironmentVariable("PANTALLA_PASSWORD");

// =========================
// CONNECTION STRING DINÁMICA
// =========================
var connectionString =
    $"server={dbHost};" +
    $"port={dbPort};" +
    $"database={dbName};" +
    $"user={dbUser};" +
    $"password={dbPassword};";

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString,
        ServerVersion.AutoDetect(connectionString))
);

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

var supportedCultures = new[] { new CultureInfo("es-AR") };

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("es-AR"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

// =========================
// SEED INICIAL
// =========================
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var passwordHasher = new PasswordHasher<Usuario>();

    context.Database.Migrate();

    // =========================
    // CREAR ROLES
    // =========================
    string[] roles = { "Admin", "Usuario", "Pantalla", "Operador" };

    foreach (var roleName in roles)
    {
        if (!context.Roles.Any(r => r.NombreRol == roleName))
        {
            context.Roles.Add(new Rol
            {
                NombreRol = roleName,
                UsuarioRoles = new List<UsuarioRol>()
            });
        }
    }

    context.SaveChanges();

    // =========================
    // CREAR ADMIN
    // =========================
    var rolAdmin = context.Roles.First(r => r.NombreRol == "Admin");

    var admin = context.Usuarios
        .Include(u => u.UsuarioRoles)
        .FirstOrDefault(u => u.Correo == adminEmail);

    if (admin == null)
    {
        admin = new Usuario
        {
            Nombre = "Administrador",
            Correo = adminEmail!,
            Contrasena = passwordHasher.HashPassword(null!, adminPassword!),
            Estado = true,
            FechaRegistro = DateTime.Now,
            UsuarioRoles = new List<UsuarioRol>()
        };

        context.Usuarios.Add(admin);
        context.SaveChanges();
    }

    if (!context.UsuarioRol.Any(ur =>
        ur.IdUsuario == admin.IdUsuario &&
        ur.IdRol == rolAdmin.IdRol))
    {
        context.UsuarioRol.Add(new UsuarioRol
        {
            IdUsuario = admin.IdUsuario,
            IdRol = rolAdmin.IdRol,
            Usuario = admin,
            Rol = rolAdmin
        });

        context.SaveChanges();
    }

    // =========================
    // CREAR USUARIO PANTALLA
    // =========================
    var rolPantalla = context.Roles.First(r => r.NombreRol == "Pantalla");

    var usuarioPantalla = context.Usuarios
        .Include(u => u.UsuarioRoles)
        .FirstOrDefault(u => u.Correo == pantallaEmail);

    if (usuarioPantalla == null)
    {
        usuarioPantalla = new Usuario
        {
            Nombre = "Pantalla",
            Correo = pantallaEmail!,
            Contrasena = passwordHasher.HashPassword(null!, pantallaPassword!),
            Estado = true,
            FechaRegistro = DateTime.Now,
            UsuarioRoles = new List<UsuarioRol>()
        };

        context.Usuarios.Add(usuarioPantalla);
        context.SaveChanges();
    }

    if (!context.UsuarioRol.Any(ur =>
        ur.IdUsuario == usuarioPantalla.IdUsuario &&
        ur.IdRol == rolPantalla.IdRol))
    {
        context.UsuarioRol.Add(new UsuarioRol
        {
            IdUsuario = usuarioPantalla.IdUsuario,
            IdRol = rolPantalla.IdRol,
            Usuario = usuarioPantalla,
            Rol = rolPantalla
        });

        context.SaveChanges();
    }

    // =========================
    // CREAR COMERCIO PARA ADMIN
    // =========================
    var comercioAdmin = context.Comercios
        .FirstOrDefault(c => c.IdUsuario == admin.IdUsuario);

    if (comercioAdmin == null)
    {
        comercioAdmin = new Comercio
        {
            IdUsuario = admin.IdUsuario,
            Usuario = admin,
            NombreComercio = "Comercio Principal",
            Descripcion = "Comercio creado automáticamente para el administrador.",
            Estado = true,
            FechaRegistro = DateTime.Now,
            Logo = null,
            Productos = new List<Producto>(),
            Configuraciones = new List<ConfiguracionPublicidad>()
        };

        context.Comercios.Add(comercioAdmin);
        context.SaveChanges();

        var config = new ConfiguracionPublicidad
        {
            IdComercio = comercioAdmin.IdComercio,
            TipoFondo = "Color",
            ColorFondo = "#FFFFFF",
            ImagenFondo = null,
            MostrarSubio = true,
            MostrarBajo = true,
            MostrarPrecioAnterior = true,
            ColorPrecioNormal = "#000000",
            ColorPrecioSubio = "#FF0000",
            ColorPrecioBajo = "#00FF00",
            TipoAnimacion = "Fade",
            DuracionAnimacionMs = 1200,
            AnimarPrecio = true,
            TiempoPorSlideMs = 8000,
            Activo = false
        };

        context.ConfiguracionPublicidad.Add(config);
        context.SaveChanges();
    }
}

app.Run();