using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServicioCatalogo.Data;
using ServicioCatalogo.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicioCatalogo.Controllers
{
    // Atributos esenciales para un controlador de API
    [Route("api/[controller]")] // Define la ruta base para este controlador (ej. /api/productos)
    [ApiController]             // Indica que este es un controlador de API, habilita características como el binding automático
    public class ProductosController : ControllerBase // Hereda de ControllerBase para controladores de API puros
    {
        private readonly CatalogoDbContext _context;

        public ProductosController(CatalogoDbContext context)
        {
            _context = context;
        }

        // GET: api/Productos
        // Obtiene todos los productos del catálogo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            if (_context.Productos == null)
            {
                return NotFound(); // Retorna 404 si la tabla de productos es null (poco probable)
            }
            return await _context.Productos.ToListAsync(); // Retorna 200 OK con la lista de productos
        }

        // GET: api/Productos/5
        // Obtiene un producto específico por su ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            if (_context.Productos == null)
            {
                return NotFound();
            }
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return NotFound(); // Retorna 404 si el producto no se encuentra
            }

            return producto; // Retorna 200 OK con el producto encontrado
        }

        // PUT: api/Productos/5
        // Actualiza un producto existente
        // Para protegerse de ataques de overposting, vea https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(int id, Producto producto)
        {
            // Verifica que el ID en la URL coincida con el ID del objeto Producto
            if (id != producto.Id)
            {
                return BadRequest(); // Retorna 400 Bad Request si los IDs no coinciden
            }

            _context.Entry(producto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExists(id))
                {
                    return NotFound(); // Retorna 404 si el producto no existe
                }
                else
                {
                    throw; // Lanza la excepción si es otro tipo de error de concurrencia
                }
            }

            return NoContent(); // Retorna 204 No Content para una actualización exitosa sin contenido
        }

        // POST: api/Productos
        // Crea un nuevo producto
        // Para protegerse de ataques de overposting, vea https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Producto>> PostProducto(Producto producto)
        {
            if (_context.Productos == null)
            {
                // Si el DbSet es null, retorna un error de servidor (500 Internal Server Error)
                return Problem("Entity set 'CatalogoDbContext.Productos'  is null.");
            }
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            // Retorna 201 CreatedAtAction con la URL del nuevo recurso creado
            // y el objeto del producto creado.
            return CreatedAtAction("GetProducto", new { id = producto.Id }, producto);
        }

        // DELETE: api/Productos/5
        // Elimina un producto por su ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            if (_context.Productos == null)
            {
                return NotFound();
            }
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound(); // Retorna 404 si el producto no existe
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return NoContent(); // Retorna 204 No Content para una eliminación exitosa sin contenido
        }

        // Método auxiliar para verificar si un producto existe
        private bool ProductoExists(int id)
        {
            return (_context.Productos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}