// Archivo: Dtos.cs (o el nombre que tenga tu archivo de DTOs en ServicioVentas)
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System; // Agregamos System para DateTime

namespace ServicioVentas.Dtos
{
    // DTO para la solicitud de creación de una nueva venta
    public class CrearVentaRequest
    {
        [Required(ErrorMessage = "El ID del cliente es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del cliente debe ser un número positivo.")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "La lista de ítems de venta es obligatoria.")]
        [MinLength(1, ErrorMessage = "Debe haber al menos un ítem en la venta.")]
        public List<ItemVentaRequest> Items { get; set; } = new List<ItemVentaRequest>();
    }

    // DTO para cada ítem dentro de la solicitud de venta
    public class ItemVentaRequest
    {
        [Required(ErrorMessage = "El ID del producto es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del producto debe ser un número positivo.")]
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Cantidad { get; set; }
    }

    // --- NUEVOS DTOS PARA COTIZACIONES ---

    // DTO para la solicitud de creación de una nueva cotización
    public class CrearCotizacionRequest
    {
        [Required(ErrorMessage = "El ID del cliente es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del cliente debe ser un número positivo.")]
        public int ClienteId { get; set; }

        // Opcional: Fecha de expiración de la cotización
        public DateTime? FechaExpiracion { get; set; }

        [Required(ErrorMessage = "La lista de ítems de cotización es obligatoria.")]
        [MinLength(1, ErrorMessage = "Debe haber al menos un ítem en la cotización.")]
        public List<ItemCotizacionRequest> Items { get; set; } = new List<ItemCotizacionRequest>();
    }

    // DTO para cada ítem dentro de la solicitud de cotización
    public class ItemCotizacionRequest
    {
        [Required(ErrorMessage = "El ID del producto es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del producto debe ser un número positivo.")]
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El precio de cotización del ítem es obligatorio.")]
        [Range(0.0, double.MaxValue, ErrorMessage = "El precio de cotización del ítem no puede ser negativo.")]
        public decimal PrecioEnCotizacion { get; set; }
    }

    // DTO para mostrar una Cotización existente (para API GET)
    public class CotizacionResponse
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        // No incluimos ClienteNombre aquí, ya que el servicio de ventas no lo tiene directamente.
        // Lo resolverá el frontend o un servicio de composición.
        public DateTime Fecha { get; set; }
        public DateTime? FechaExpiracion { get; set; }
        public decimal TotalCotizacion { get; set; }
        public bool EsVigente { get; set; }
        public List<DetalleCotizacionResponse> Detalles { get; set; } = new List<DetalleCotizacionResponse>();
    }

    // DTO para mostrar un DetalleCotizacion existente (para API GET)
    public class DetalleCotizacionResponse
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        // No incluimos ProductoNombre aquí, misma razón que ClienteNombre.
        public int Cantidad { get; set; }
        public decimal PrecioEnCotizacion { get; set; }
    }
}