using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Para el atributo [ForeignKey]

namespace ServicioVentas.Models // Asegúrate de que el namespace sea ServicioVentas.Models
{
    public class DetalleVenta
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El precio en venta es obligatorio.")]
        [Range(0.0, double.MaxValue, ErrorMessage = "El precio en venta no puede ser negativo.")]
        public decimal PrecioEnVenta { get; set; }

        // Relación: Este detalle pertenece a una venta
        [Required(ErrorMessage = "El ID de la venta es obligatorio.")]
        public int VentaId { get; set; }
        [ForeignKey("VentaId")]
        public Venta Venta { get; set; } = null!; // <--- ¡MODIFICADO AQUÍ!

        // Relación: Este detalle corresponde a un producto
        // Solo incluiremos el ID del producto, ya que la información del producto
        // se obtendrá del ServicioCatalogo.
        [Required(ErrorMessage = "El ID del producto es obligatorio.")]
        public int ProductoId { get; set; }
        // No incluimos 'Producto Producto' aquí para mantener la independencia de los microservicios.
        // Los detalles del producto se obtendrán haciendo una llamada al ServicioCatalogo.
    }
}