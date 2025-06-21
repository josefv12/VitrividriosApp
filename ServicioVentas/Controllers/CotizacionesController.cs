// Archivo: ServicioVentas/Controllers/CotizacionesController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServicioVentas.Data;
using ServicioVentas.Dtos; // Para los DTOs de solicitud/respuesta
using ServicioVentas.Models; // Para los modelos de base de datos
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicioVentas.Controllers
{
    [Route("api/[controller]")] // Define la ruta base para este controlador (ej. /api/Cotizaciones)
    [ApiController] // Indica que es un controlador de API
    public class CotizacionesController : ControllerBase
    {
        private readonly VentasDbContext _context;

        public CotizacionesController(VentasDbContext context)
        {
            _context = context;
        }

        // GET: api/Cotizaciones
        // Obtiene una lista de todas las cotizaciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CotizacionResponse>>> GetCotizaciones()
        {
            var cotizaciones = await _context.Cotizaciones
                                             .Include(c => c.Detalles) // Incluye los detalles de cada cotización
                                             .ToListAsync();

            // Mapea las entidades de modelo a DTOs de respuesta
            var cotizacionDtos = cotizaciones.Select(c => new CotizacionResponse
            {
                Id = c.Id,
                ClienteId = c.ClienteId,
                Fecha = c.Fecha,
                FechaExpiracion = c.FechaExpiracion,
                TotalCotizacion = c.TotalCotizacion,
                EsVigente = c.EsVigente,
                Detalles = c.Detalles.Select(d => new DetalleCotizacionResponse
                {
                    Id = d.Id,
                    ProductoId = d.ProductoId,
                    Cantidad = d.Cantidad,
                    PrecioEnCotizacion = d.PrecioEnCotizacion
                }).ToList()
            }).ToList();

            return Ok(cotizacionDtos);
        }

        // GET: api/Cotizaciones/5
        // Obtiene una cotización específica por su ID
        [HttpGet("{id}")]
        public async Task<ActionResult<CotizacionResponse>> GetCotizacion(int id)
        {
            var cotizacion = await _context.Cotizaciones
                                           .Include(c => c.Detalles)
                                           .FirstOrDefaultAsync(c => c.Id == id);

            if (cotizacion == null)
            {
                return NotFound(); // Retorna 404 si la cotización no se encuentra
            }

            // Mapea la entidad a DTO de respuesta
            var cotizacionDto = new CotizacionResponse
            {
                Id = cotizacion.Id,
                ClienteId = cotizacion.ClienteId,
                Fecha = cotizacion.Fecha,
                FechaExpiracion = cotizacion.FechaExpiracion,
                TotalCotizacion = cotizacion.TotalCotizacion,
                EsVigente = cotizacion.EsVigente,
                Detalles = cotizacion.Detalles.Select(d => new DetalleCotizacionResponse
                {
                    Id = d.Id,
                    ProductoId = d.ProductoId,
                    Cantidad = d.Cantidad,
                    PrecioEnCotizacion = d.PrecioEnCotizacion
                }).ToList()
            };

            return Ok(cotizacionDto);
        }

        // POST: api/Cotizaciones
        // Crea una nueva cotización
        [HttpPost]
        public async Task<ActionResult<CotizacionResponse>> PostCotizacion(CrearCotizacionRequest request)
        {
            // Validaciones adicionales de negocio
            if (!request.Items.Any())
            {
                return BadRequest("La cotización debe contener al menos un producto.");
            }

            // Aquí deberías realizar validaciones de stock o existencia de productos/clientes
            // Esto implicaría hacer llamadas HTTP a ServicioCatalogo y ServicioClientes
            // Para mantener este ejemplo conciso, asumiremos que los IDs son válidos
            // y que el stock no se descuenta para cotizaciones (solo para ventas).

            decimal totalCotizacionCalculado = 0;
            foreach (var itemRequest in request.Items)
            {
                // Para cotizaciones, el precio ya viene en el request (PrecioEnCotizacion)
                // En un escenario real, aquí podrías verificar que el ProductoId existe
                // y obtener el precio actual del ServicioCatalogo para evitar manipulaciones.
                // Sin embargo, para fines de la cotización, se usa el precio del request.
                totalCotizacionCalculado += itemRequest.Cantidad * itemRequest.PrecioEnCotizacion;
            }

            var cotizacion = new Cotizacion
            {
                ClienteId = request.ClienteId,
                Fecha = DateTime.Now,
                FechaExpiracion = request.FechaExpiracion,
                TotalCotizacion = totalCotizacionCalculado,
                EsVigente = true, // Una nueva cotización es vigente por defecto
                Detalles = request.Items.Select(itemRequest => new DetalleCotizacion
                {
                    ProductoId = itemRequest.ProductoId,
                    Cantidad = itemRequest.Cantidad,
                    PrecioEnCotizacion = itemRequest.PrecioEnCotizacion
                }).ToList()
            };

            _context.Cotizaciones.Add(cotizacion);
            await _context.SaveChangesAsync();

            // Mapea la entidad creada a DTO de respuesta para el cliente
            var cotizacionResponse = new CotizacionResponse
            {
                Id = cotizacion.Id,
                ClienteId = cotizacion.ClienteId,
                Fecha = cotizacion.Fecha,
                FechaExpiracion = cotizacion.FechaExpiracion,
                TotalCotizacion = cotizacion.TotalCotizacion,
                EsVigente = cotizacion.EsVigente,
                Detalles = cotizacion.Detalles.Select(d => new DetalleCotizacionResponse
                {
                    Id = d.Id,
                    ProductoId = d.ProductoId,
                    Cantidad = d.Cantidad,
                    PrecioEnCotizacion = d.PrecioEnCotizacion
                }).ToList()
            };

            // Retorna una respuesta 201 Created con la URL de la nueva cotización
            return CreatedAtAction(nameof(GetCotizacion), new { id = cotizacion.Id }, cotizacionResponse);
        }

        // Opcional: PUT y DELETE para cotizaciones
        // ... puedes añadir métodos para actualizar y eliminar cotizaciones si es necesario.
        // Por ejemplo, un PUT para marcar una cotización como no vigente, o para editarla.

        // DELETE: api/Cotizaciones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCotizacion(int id)
        {
            var cotizacion = await _context.Cotizaciones.FindAsync(id);
            if (cotizacion == null)
            {
                return NotFound();
            }

            _context.Cotizaciones.Remove(cotizacion);
            await _context.SaveChangesAsync();

            return NoContent(); // Retorna 204 No Content para una eliminación exitosa
        }
    }
}