namespace VitrividriosApp.Web.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal TotalVenta { get; set; }

        // Relación: Una venta pertenece a un cliente
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        // Relación: Una venta tiene muchos detalles
        public List<DetalleVenta> Detalles { get; set; }
    }
}