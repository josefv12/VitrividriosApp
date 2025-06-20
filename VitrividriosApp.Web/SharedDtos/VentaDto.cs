// VitrividriosApp.Web/SharedDtos/VentaDto.cs
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace VitrividriosApp.Web.SharedDtos
{
    // DTO para mostrar los detalles de una Venta en la UI
    public class VentaDto
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string ClienteNombre { get; set; } = string.Empty; // Para mostrar el nombre del cliente en la UI
        public DateTime Fecha { get; set; }
        public decimal TotalVenta { get; set; }
        public List<DetalleVentaDto> Detalles { get; set; } = new List<DetalleVentaDto>();
    }

    // DTO para los detalles de cada ítem dentro de una Venta
    public class DetalleVentaDto
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; } = string.Empty; // Para mostrar el nombre del producto en la UI
        public int Cantidad { get; set; }
        public decimal PrecioEnVenta { get; set; } // El precio unitario al que se vendió
    }

    // DTO para la solicitud de creación de una nueva venta desde la UI
    // Este DTO mapea a CrearVentaRequest en ServicioVentas
    public class CrearVentaRequestDto
    {
        [Required(ErrorMessage = "El ID del cliente es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un cliente.")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "La lista de ítems de venta es obligatoria.")]
        [MinLength(1, ErrorMessage = "Debe haber al menos un ítem en la venta.")]
        public List<ItemVentaRequestDto> Items { get; set; } = new List<ItemVentaRequestDto>();
    }

    // DTO para cada ítem dentro de la solicitud de venta desde la UI
    public class ItemVentaRequestDto
    {
        [Required(ErrorMessage = "El ID del producto es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un producto.")]
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Cantidad { get; set; }
    }
}