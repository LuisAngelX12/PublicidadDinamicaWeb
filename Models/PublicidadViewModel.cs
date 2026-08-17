namespace PublicidadDinamicaWeb.Models
{
    public class PublicidadViewModel
    {
        public IEnumerable<Producto>? Productos { get; set; }
        public ConfiguracionPublicidad? Configuracion { get; set; }
    }
}