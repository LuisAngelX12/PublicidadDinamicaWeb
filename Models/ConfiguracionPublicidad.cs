using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PublicidadDinamicaWeb.Models
{
    public class ConfiguracionPublicidad
    {
        [Key]
        public int IdConfiguracion { get; set; }

        public int? IdComercio { get; set; }

        [ForeignKey(nameof(IdComercio))]
        public Comercio? Comercio { get; set; }

        /* FONDO */
        public string TipoFondo { get; set; } = "color"; // color | imagen | gradiente
        public string? ColorFondo { get; set; } = "#0f1115";
        public string? ImagenFondo { get; set; }

        /* VARIACIONES */
        public bool MostrarSubio { get; set; } = true;
        public bool MostrarBajo { get; set; } = true;

        /* PRECIO */
        public bool MostrarPrecioAnterior { get; set; } = true;
        public string ColorPrecioNormal { get; set; } = "#ffffff";
        public string ColorPrecioSubio { get; set; } = "#ff6b6b";
        public string ColorPrecioBajo { get; set; } = "#2eff8b";

        /* ANIMACIONES */
        public string TipoAnimacion { get; set; } = "fade"; // fade | slide | zoom
        public int DuracionAnimacionMs { get; set; } = 1200;
        public bool AnimarPrecio { get; set; } = true;

        /* TIEMPOS */
        public int TiempoPorSlideMs { get; set; } = 8000;

        /* ESTADO */
        public bool Activo { get; set; } = true;


        [NotMapped] // No se guarda en la BD
        public double TiempoPorSlideSeg
        {
            get => TiempoPorSlideMs / 1000.0; // Mostrar en segundos
            set => TiempoPorSlideMs = (int)(value * 1000); // Guardar en ms
        }

        [NotMapped]
        public double DuracionAnimacionSeg
        {
            get => DuracionAnimacionMs / 1000.0;
            set => DuracionAnimacionMs = (int)(value * 1000);
        }
    }
}