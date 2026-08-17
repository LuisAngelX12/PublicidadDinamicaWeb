namespace PublicidadDinamicaWeb.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class HistorialPrecio
    {
        [Key]
        public int IdHistorial { get; set; }

        public int IdProducto { get; set; }

        [ForeignKey(nameof(IdProducto))]
        public required Producto Producto { get; set; }

        public decimal PrecioAnterior { get; set; }
        public decimal PrecioNuevo { get; set; }

        public DateTime FechaCambio { get; set; } = DateTime.UtcNow;
    }
}