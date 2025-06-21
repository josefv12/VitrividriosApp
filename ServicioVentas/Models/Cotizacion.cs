// Archivo: ServicioVentas/Models/Cotizacion.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicioVentas.Models
{
    public class Cotizacion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Fecha { get; set; } = DateTime.Now; // Fecha de creación de la cotización

        public DateTime? FechaExpiracion { get; set; } // Fecha opcional de expiración

        [Required]
        [Range(0.0, double.MaxValue, ErrorMessage = "El total de la cotización no puede ser negativo.")]
        public decimal TotalCotizacion { get; set; }

        [Required]
        public bool EsVigente { get; set; } = true; // Por defecto, una nueva cotización es vigente

        // Relación: Una cotización pertenece a un cliente
        // Solo incluimos el ID del cliente para mantener la independencia de microservicios.
        [Required(ErrorMessage = "El ID del cliente es obligatorio.")]
        public int ClienteId { get; set; }

        // Relación: Una cotización tiene muchos detalles de cotización
        public List<DetalleCotizacion> Detalles { get; set; } = new List<DetalleCotizacion>();
    }
}