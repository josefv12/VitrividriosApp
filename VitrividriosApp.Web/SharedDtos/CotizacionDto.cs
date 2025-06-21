// Archivo: VitrividriosApp.Web/SharedDtos/CotizacionDto.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VitrividriosApp.Web.SharedDtos
{
    // DTO para mostrar los detalles de una Cotización en la UI
    public class CotizacionDto
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string ClienteNombre { get; set; } = string.Empty; // Para mostrar el nombre del cliente
        public DateTime Fecha { get; set; }
        public DateTime? FechaExpiracion { get; set; } // Fecha opcional de expiración de la cotización
        public decimal TotalCotizacion { get; set; }
        public bool EsVigente { get; set; } // Indica si la cotización aún es válida
        public List<DetalleCotizacionDto> Detalles { get; set; } = new List<DetalleCotizacionDto>();
    }

    // DTO para los detalles de cada ítem dentro de una Cotización
    public class DetalleCotizacionDto
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; } = string.Empty; // Para mostrar el nombre del producto
        public int Cantidad { get; set; }
        public decimal PrecioEnCotizacion { get; set; } // El precio unitario al que se cotizó
    }

    // DTO para la solicitud de creación de una nueva cotización desde la UI
    public class CrearCotizacionRequestDto
    {
        [Required(ErrorMessage = "El ID del cliente es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un cliente.")]
        public int ClienteId { get; set; }

        // La fecha de expiración es opcional para la creación, pero se puede añadir validación si es requerida por negocio
        public DateTime? FechaExpiracion { get; set; }

        [Required(ErrorMessage = "La lista de ítems de cotización es obligatoria.")]
        [MinLength(1, ErrorMessage = "Debe haber al menos un ítem en la cotización.")]
        public List<ItemCotizacionRequestDto> Items { get; set; } = new List<ItemCotizacionRequestDto>();
    }

    // DTO para cada ítem dentro de la solicitud de cotización desde la UI
    public class ItemCotizacionRequestDto
    {
        [Required(ErrorMessage = "El ID del producto es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un producto.")]
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Cantidad { get; set; }

        // El precio al que se cotizó el producto en ese momento.
        // Se llenará en la UI al añadir el producto, igual que en las ventas.
        [Required(ErrorMessage = "El precio de cotización del ítem es obligatorio.")]
        [Range(0.0, double.MaxValue, ErrorMessage = "El precio de cotización del ítem no puede ser negativo.")]
        public decimal PrecioEnCotizacion { get; set; }
    }
}