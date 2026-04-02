namespace PublicidadDinamicaWeb.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Producto
    {
        [Key]
        public int IdProducto { get; set; }

        public int IdComercio { get; set; }

        [ForeignKey(nameof(IdComercio))]
        public Comercio Comercio { get; set; }

        [Display(Name = "Categoria")]
        public int IdCategoria { get; set; }

        [ForeignKey(nameof(IdCategoria))]
        public Categoria Categoria { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio")]
        [Display(Name = "Nombre del producto")]
        public required string NombreProducto { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        public required string Descripcion { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        [Display(Name = "Precio")]
        public decimal PrecioActual { get; set; }

        public string? Imagen { get; set; }

        public required bool Estado { get; set; } = true;

        public required ICollection<HistorialPrecio> HistorialPrecios { get; set; }
        public ICollection<Anuncio> Anuncios { get; set; } = new List<Anuncio>();
    }
}