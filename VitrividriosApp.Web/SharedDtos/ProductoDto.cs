// VitrividriosApp.Web/SharedDtos/ProductoDto.cs
using System.ComponentModel.DataAnnotations; // Necesario para DataAnnotations

namespace VitrividriosApp.Web.SharedDtos
{
    // Este DTO representa cómo recibimos un Producto desde ServicioCatalogo
    // y cómo lo mostramos/editamos en la UI.
    public class ProductoDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty; // Inicializado para evitar advertencias CS8618

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres.")]
        public string Descripcion { get; set; } = string.Empty; // Inicializado para evitar advertencias CS8618

        [Required(ErrorMessage = "El precio unitario es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio unitario debe ser mayor que cero.")]
        public decimal PrecioUnitario { get; set; }

        [Required(ErrorMessage = "El precio mayorista es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio mayorista debe ser mayor que cero.")]
        public decimal PrecioMayorista { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
        public int Stock { get; set; }
    }

    // DTO para enviar datos cuando creamos o actualizamos un producto.
    // En este caso, es idéntico a ProductoDto excepto por el Id,
    // ya que la API espera un cuerpo sin Id para POST y con Id en la ruta para PUT.
    public class CrearOActualizarProductoDto
    {
        // No incluimos el Id aquí para que sea más fácil usarlo en un formulario de creación/actualización.
        // El Id se manejará a través del [BindProperty] y asp-for en la vista.
        public int Id { get; set; } // Lo añadimos aquí para que el BindProperty lo capture

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres.")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio unitario es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio unitario debe ser mayor que cero.")]
        public decimal PrecioUnitario { get; set; }

        [Required(ErrorMessage = "El precio mayorista es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio mayorista debe ser mayor que cero.")]
        public decimal PrecioMayorista { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
        public int Stock { get; set; }
    }
}