namespace PublicidadDinamicaWeb.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Anuncio
    {
        [Key]
        public int IdAnuncio { get; set; }

        [Display(Name = "Producto")]
        public int IdProducto { get; set; }

        [ForeignKey(nameof(IdProducto))]
        public required Producto Producto { get; set; }

        [Required]
        public required string Titulo { get; set; }

        [Display(Name = "Texto Promocional")]
        public required string TextoPromocional { get; set; }

        [Display(Name = "Fecha Inicio")]
        public DateTime? FechaInicio { get; set; }

        [Display(Name = "Fecha Final")]
        public DateTime? FechaFin { get; set; }

        public bool Estado { get; set; } = true;
    }
}