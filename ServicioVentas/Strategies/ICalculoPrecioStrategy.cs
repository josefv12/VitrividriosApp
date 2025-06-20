namespace ServicioVentas.Strategies
{
    /// <summary>
    /// Interfaz que define el contrato para las diferentes estrategias de cálculo de precios.
    /// </summary>
    public interface ICalculoPrecioStrategy
    {
        /// <summary>
        /// Calcula el precio total de un producto basándose en un precio base y una cantidad.
        /// </summary>
        /// <param name="precioBase">El precio unitario base del producto.</param>
        /// <param name="cantidad">La cantidad de unidades del producto.</param>
        /// <returns>El precio total calculado para la cantidad especificada.</returns>
        decimal CalcularPrecio(decimal precioBase, int cantidad);
    }
}