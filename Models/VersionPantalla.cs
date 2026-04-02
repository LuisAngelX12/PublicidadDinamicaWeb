namespace PublicidadDinamicaWeb.Models
{
    using System.ComponentModel.DataAnnotations;

    namespace PublicidadDinamicaWeb.Models
    {
        public class VersionPantalla
        {
            [Key]
            public int Id { get; set; }

            public int Valor { get; set; } = 1;
        }
    }
}