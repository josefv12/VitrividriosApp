namespace VitrividriosApp.Web.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal PrecioMayorista { get; set; }
        public int Stock { get; set; }
    }
}
