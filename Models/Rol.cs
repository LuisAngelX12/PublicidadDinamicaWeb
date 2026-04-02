namespace PublicidadDinamicaWeb.Models
{
    using System.ComponentModel.DataAnnotations;
    public class Rol
    {
        [Key]
        public int IdRol { get; set; }

        [Required]
        public required string NombreRol { get; set; }

        public required ICollection<UsuarioRol> UsuarioRoles { get; set; }
    }
}