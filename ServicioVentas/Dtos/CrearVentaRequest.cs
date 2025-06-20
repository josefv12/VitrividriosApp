using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
}