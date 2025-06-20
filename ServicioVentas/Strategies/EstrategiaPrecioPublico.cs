namespace ServicioVentas.Strategies
{
    /// <summary>
    /// Implementación de ICalculoPrecioStrategy para clientes de precio público (minorista).
    /// </summary>
    public class EstrategiaPrecioPublico : ICalculoPrecioStrategy
    {
        /// <summary>
        /// Calcula el precio utilizando el precio base directamente.
        /// </summary>
        /// <param name="precioBase">El precio unitario público del producto.</param>
        /// <param name="cantidad">La cantidad de unidades del producto.</param>
        /// <returns>El precio total calculado (precioBase * cantidad).</returns>
        public decimal CalcularPrecio(decimal precioBase, int cantidad)
        {
            // Para el público general, el precio es simplemente el precio base por la cantidad.
            return precioBase * cantidad;
        }
    }
}