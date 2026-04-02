namespace PublicidadDinamicaWeb.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class UsuarioRol
    {
        [Key]
        public int IdUsuarioRol { get; set; }

        public int IdUsuario { get; set; }

        [ForeignKey(nameof(IdUsuario))]
        public required Usuario Usuario { get; set; }

        public int IdRol { get; set; }
        public required Rol Rol { get; set; }
    }
}