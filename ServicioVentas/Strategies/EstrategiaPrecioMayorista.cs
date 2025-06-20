namespace ServicioVentas.Strategies
{
    /// <summary>
    /// Implementación de ICalculoPrecioStrategy para clientes mayoristas.
    /// </summary>
    public class EstrategiaPrecioMayorista : ICalculoPrecioStrategy
    {
        /// <summary>
        /// Calcula el precio utilizando el precio mayorista del producto.
        /// </summary>
        /// <param name="precioMayorista">El precio unitario mayorista del producto.</param>
        /// <param name="cantidad">La cantidad de unidades del producto.</param>
        /// <returns>El precio total calculado (precioMayorista * cantidad).</returns>
        public decimal CalcularPrecio(decimal precioMayorista, int cantidad)
        {
            // Para clientes mayoristas, se usa el precioMayorista por la cantidad.
            return precioMayorista * cantidad;
        }
    }
}