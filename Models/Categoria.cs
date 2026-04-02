namespace PublicidadDinamicaWeb.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Categoria
    {
        [Key]
        public int IdCategoria { get; set; }

        [Required]
        [Display(Name = "Nombre de categoría")]
        public required string NombreCategoria { get; set; }
    }
}