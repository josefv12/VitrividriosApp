// Archivo: ServicioVentas/Models/DetalleCotizacion.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicioVentas.Models
{
    public class DetalleCotizacion
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El precio en cotización es obligatorio.")]
        [Range(0.0, double.MaxValue, ErrorMessage = "El precio en cotización no puede ser negativo.")]
        public decimal PrecioEnCotizacion { get; set; }

        // Relación: Este detalle pertenece a una cotización
        [Required(ErrorMessage = "El ID de la cotización es obligatorio.")]
        public int CotizacionId { get; set; }
        [ForeignKey("CotizacionId")]
        public Cotizacion Cotizacion { get; set; } = null!; // Propiedad de navegación

        // Relación: Este detalle corresponde a un producto
        // Solo incluimos el ID del producto para mantener la independencia de microservicios.
        [Required(ErrorMessage = "El ID del producto es obligatorio.")]
        public int ProductoId { get; set; }
        // No incluimos 'Producto Producto' aquí para mantener la independencia.
    }
}