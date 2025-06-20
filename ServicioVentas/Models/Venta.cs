using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ServicioVentas.Models // Asegúrate de que el namespace sea ServicioVentas.Models
{
    public class Venta
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Fecha { get; set; } = DateTime.Now; // Valor por defecto

        [Required]
        [Range(0.0, double.MaxValue, ErrorMessage = "El total de la venta no puede ser negativo.")]
        public decimal TotalVenta { get; set; }

        // Relación: Una venta pertenece a un cliente
        // No incluiremos el objeto Cliente aquí, solo su ID,
        // ya que la información del cliente se obtendrá del ServicioClientes.
        [Required(ErrorMessage = "El ID del cliente es obligatorio.")]
        public int ClienteId { get; set; }

        // Relación: Una venta tiene muchos detalles
        public List<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
    }
}