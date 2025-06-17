namespace VitrividriosApp.Web.Models
{
    public class DetalleVenta
    {
        public int Id { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioEnVenta { get; set; }

        // Relación: Este detalle pertenece a una venta
        public int VentaId { get; set; }
        public Venta Venta { get; set; }

        // Relación: Este detalle corresponde a un producto
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
    }
}