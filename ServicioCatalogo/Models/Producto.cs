using System.ComponentModel.DataAnnotations;

namespace ServicioCatalogo.Models
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public required string Nombre { get; set; } // Modificador 'required' añadido

        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres.")]
        public required string Descripcion { get; set; } // Modificador 'required' añadido

        [Required(ErrorMessage = "El precio unitario es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio unitario debe ser mayor que cero.")]
        public decimal PrecioUnitario { get; set; }

        [Required(ErrorMessage = "El precio mayorista es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio mayorista debe ser mayor que cero.")]
        public decimal PrecioMayorista { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
        public int Stock { get; set; } // Representa el inventario de este producto
    }
}