namespace PublicidadDinamicaWeb.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Comercio
    {
        [Key]
        public int IdComercio { get; set; }

        [Display(Name = "Usuario")]
        public int IdUsuario { get; set; }

        [ForeignKey(nameof(IdUsuario))]
        public required Usuario Usuario { get; set; }

        [Required(ErrorMessage = "El nombre del comercio es obligatorio")]
        [Display(Name = "Nombre del comercio")]
        public required string NombreComercio { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        public required string Descripcion { get; set; }

        public bool Estado { get; set; } = true;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public string? Logo { get; set; }

        public required ICollection<Producto> Productos { get; set; }
        public ICollection<ConfiguracionPublicidad> Configuraciones { get; set; }
    }
}