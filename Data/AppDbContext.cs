namespace PublicidadDinamicaWeb.Data
{
    using Microsoft.EntityFrameworkCore;
    using PublicidadDinamicaWeb.Models;
    using PublicidadDinamicaWeb.Models.PublicidadDinamicaWeb.Models;

    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<UsuarioRol> UsuarioRol { get; set; }
        public DbSet<Comercio> Comercios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<HistorialPrecio> HistorialPrecios { get; set; }
        public DbSet<Anuncio> Anuncios { get; set; }
        public DbSet<ConfiguracionPublicidad> ConfiguracionPublicidad { get; set; }
        public DbSet<VersionPantalla> VersionPantalla { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UsuarioRol>()
                .HasKey(ur => ur.IdUsuarioRol);

            modelBuilder.Entity<UsuarioRol>()
                .HasOne(ur => ur.Usuario)
                .WithMany(u => u.UsuarioRoles)
                .HasForeignKey(ur => ur.IdUsuario);

            modelBuilder.Entity<UsuarioRol>()
                .HasOne(ur => ur.Rol)
                .WithMany(r => r.UsuarioRoles)
                .HasForeignKey(ur => ur.IdRol);
        }

        public override int SaveChanges()
        {
            ConvertirFechasUtc();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ConvertirFechasUtc();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void ConvertirFechasUtc()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                foreach (var property in entry.Properties)
                {
                    if (property.CurrentValue is DateTime dt)
                    {
                        if (dt.Kind == DateTimeKind.Local)
                        {
                            property.CurrentValue = dt.ToUniversalTime();
                        }
                        else if (dt.Kind == DateTimeKind.Unspecified)
                        {
                            property.CurrentValue = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                        }
                    }
                }
            }
        }
    }
}