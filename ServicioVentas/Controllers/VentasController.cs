using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServicioVentas.Data; // Para VentasDbContext
using ServicioVentas.Dtos; // Para los DTOs de venta
using ServicioVentas.Models;
using ServicioVentas.Strategies; // Necesario para las estrategias de precio
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http; // Para IHttpClientFactory
using System.Threading.Tasks;
using System.Text.Json; // Para serialización/deserialización JSON
using System.Text;     // Para StringContent

namespace ServicioVentas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly VentasDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly EstrategiaPrecioPublico _estrategiaPrecioPublico;
        private readonly EstrategiaPrecioMayorista _estrategiaPrecioMayorista;

        // Constructor con inyección de dependencias
        public VentasController(
            VentasDbContext context,
            IHttpClientFactory httpClientFactory,
            EstrategiaPrecioPublico estrategiaPrecioPublico,
            EstrategiaPrecioMayorista estrategiaPrecioMayorista)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _estrategiaPrecioPublico = estrategiaPrecioPublico;
            _estrategiaPrecioMayorista = estrategiaPrecioMayorista;
        }

        // DTOs auxiliares para recibir datos de otros microservicios
        // (Podrían estar en un proyecto compartido o definirse localmente si son específicos para la comunicación interna)
        private class ProductoResponseDto
        {
            public int Id { get; set; }
            public string Nombre { get; set; } = null!; 
            public string Descripcion { get; set; } = null!; 
            public decimal PrecioUnitario { get; set; }
            public decimal PrecioMayorista { get; set; }
            public int Stock { get; set; }
        }

        private class ClienteResponseDto
        {
            public int Id { get; set; }
            public string Nombre { get; set; } = null!; 
            public bool EsMayorista { get; set; }
        }

        // POST: api/Ventas
        // Crea y registra una nueva venta
        [HttpPost]
        public async Task<IActionResult> PostVenta([FromBody] CrearVentaRequest request)
        {
            // 1. Validar el DTO de entrada
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Retorna 400 Bad Request con errores de validación
            }

            // --- Orquestación con ServicioClientes ---
            // 2. Obtener información del cliente desde ServicioClientes
            var clienteHttpClient = _httpClientFactory.CreateClient("ServicioClientes");
            ClienteResponseDto cliente;
            try
            {
                var clienteResponse = await clienteHttpClient.GetAsync($"api/Clientes/{request.ClienteId}");
                if (!clienteResponse.IsSuccessStatusCode)
                {
                    if (clienteResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                        return NotFound($"Cliente con ID {request.ClienteId} no encontrado.");
                    else
                        return StatusCode((int)clienteResponse.StatusCode, $"Error al obtener cliente: {await clienteResponse.Content.ReadAsStringAsync()}");
                }
                cliente = await clienteResponse.Content.ReadFromJsonAsync<ClienteResponseDto>();
                if (cliente == null)
                {
                    return BadRequest("Datos de cliente inválidos recibidos.");
                }
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error de comunicación con ServicioClientes: {ex.Message}");
            }

            // --- Orquestación con ServicioCatalogo ---
            var catalogoHttpClient = _httpClientFactory.CreateClient("ServicioCatalogo");
            var productosEnVenta = new Dictionary<int, ProductoResponseDto>();
            var detallesVenta = new List<DetalleVenta>();
            decimal totalVenta = 0;

            foreach (var item in request.Items)
            {
                ProductoResponseDto producto;
                try
                {
                    var productoResponse = await catalogoHttpClient.GetAsync($"api/Productos/{item.ProductoId}");
                    if (!productoResponse.IsSuccessStatusCode)
                    {
                        if (productoResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                            return NotFound($"Producto con ID {item.ProductoId} no encontrado.");
                        else
                            return StatusCode((int)productoResponse.StatusCode, $"Error al obtener producto: {await productoResponse.Content.ReadAsStringAsync()}");
                    }
                    producto = await productoResponse.Content.ReadFromJsonAsync<ProductoResponseDto>();
                    if (producto == null)
                    {
                        return BadRequest($"Datos de producto inválidos recibidos para ID {item.ProductoId}.");
                    }
                }
                catch (HttpRequestException ex)
                {
                    return StatusCode(500, $"Error de comunicación con ServicioCatalogo al obtener producto: {ex.Message}");
                }

                // 3. Verificar Stock
                if (producto.Stock < item.Cantidad)
                {
                    return BadRequest($"Stock insuficiente para el producto '{producto.Nombre}'. Disponible: {producto.Stock}, Solicitado: {item.Cantidad}.");
                }

                productosEnVenta.Add(producto.Id, producto); // Almacenar para futuras operaciones (ej. actualización de stock)

                // 4. Seleccionar la estrategia de precio y calcular
                ICalculoPrecioStrategy estrategia;
                decimal precioUnitarioAplicado;

                if (cliente.EsMayorista)
                {
                    estrategia = _estrategiaPrecioMayorista;
                    precioUnitarioAplicado = producto.PrecioMayorista; // Usamos el precio mayorista del producto
                }
                else
                {
                    estrategia = _estrategiaPrecioPublico;
                    precioUnitarioAplicado = producto.PrecioUnitario; // Usamos el precio unitario del producto
                }

                // Calcular el subtotal del ítem utilizando la estrategia
                decimal subtotalItem = estrategia.CalcularPrecio(precioUnitarioAplicado, item.Cantidad);
                totalVenta += subtotalItem;

                // 5. Crear DetalleVenta
                detallesVenta.Add(new DetalleVenta
                {
                    ProductoId = item.ProductoId,
                    Cantidad = item.Cantidad,
                    // Asegurarse de que el PrecioEnVenta refleje el precio unitario usado, no el subtotal
                    PrecioEnVenta = precioUnitarioAplicado
                });
            }

            // 6. Crear Venta
            var nuevaVenta = new Venta
            {
                ClienteId = request.ClienteId,
                Fecha = DateTime.Now,
                TotalVenta = totalVenta,
                Detalles = detallesVenta
            };

            // --- Guardar Venta en ServicioVentasDb ---
            // 7. Guardar la venta y sus detalles en la base de datos de Ventas
            try
            {
                _context.Ventas.Add(nuevaVenta);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Error al guardar la venta en la base de datos: {ex.Message}");
            }


            // --- Actualizar Stock en ServicioCatalogo ---
            // 8. Actualizar el stock en ServicioCatalogo para cada producto vendido
            foreach (var item in request.Items)
            {
                var productoAActualizar = productosEnVenta[item.ProductoId];
                productoAActualizar.Stock -= item.Cantidad; // Decrementar el stock

                try
                {
                    var updateContent = new StringContent(
                        JsonSerializer.Serialize(productoAActualizar),
                        Encoding.UTF8,
                        "application/json");

                    var updateResponse = await catalogoHttpClient.PutAsync($"api/Productos/{productoAActualizar.Id}", updateContent);

                    if (!updateResponse.IsSuccessStatusCode)
                    {
                        // IMPORTANTE: Manejar errores aquí. En un sistema real, esto podría requerir
                        // una compensación (cancelar la venta, revertir stock, etc.) o un sistema de reintentos.
                        // Por ahora, simplemente retornamos un error.
                        return StatusCode((int)updateResponse.StatusCode,
                                        $"Error al actualizar stock del producto {productoAActualizar.Nombre} (ID: {productoAActualizar.Id}): {await updateResponse.Content.ReadAsStringAsync()}");
                    }
                }
                catch (HttpRequestException ex)
                {
                    return StatusCode(500, $"Error de comunicación con ServicioCatalogo al actualizar stock: {ex.Message}");
                }
            }

            // 9. Retornar respuesta exitosa
            return CreatedAtAction("GetVenta", new { id = nuevaVenta.Id }, nuevaVenta);
        }

        // Opcional: Añadir un endpoint GET para obtener los detalles de una venta
        // para que CreatedAtAction pueda funcionar y para depuración.
        [HttpGet("{id}")]
        public async Task<ActionResult<Venta>> GetVenta(int id)
        {
            if (_context.Ventas == null)
            {
                return NotFound();
            }
            // Incluir los detalles de la venta para una vista completa
            var venta = await _context.Ventas
                                    .Include(v => v.Detalles)
                                    .FirstOrDefaultAsync(v => v.Id == id);

            if (venta == null)
            {
                return NotFound();
            }

            return venta;
        }

        // Opcional: Endpoint GET para obtener todas las ventas (útil para el historial)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Venta>>> GetVentas()
        {
            if (_context.Ventas == null)
            {
                return NotFound();
            }
            return await _context.Ventas.Include(v => v.Detalles).ToListAsync();
        }
    }
}