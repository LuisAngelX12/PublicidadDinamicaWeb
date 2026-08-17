namespace PublicidadDinamicaWeb.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required]
        public required string Nombre { get; set; }

        [Required, EmailAddress]
        public required string Correo { get; set; }

        [Required]
        public required string Contrasena { get; set; }

        public bool Estado { get; set; } = true;

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        public required ICollection<UsuarioRol> UsuarioRoles { get; set; }
    }
}