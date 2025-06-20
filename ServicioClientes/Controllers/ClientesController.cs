using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServicioClientes.Data;
using ServicioClientes.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServicioClientes.Controllers
{
    // Atributos esenciales para un controlador de API
    [Route("api/[controller]")] // Define la ruta base para este controlador (ej. /api/clientes)
    [ApiController]             // Indica que este es un controlador de API
    public class ClientesController : ControllerBase // Hereda de ControllerBase para controladores de API puros
    {
        private readonly ClientesDbContext _context;

        public ClientesController(ClientesDbContext context)
        {
            _context = context;
        }

        // GET: api/Clientes
        // Obtiene todos los clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
        {
            if (_context.Clientes == null)
            {
                return NotFound();
            }
            return await _context.Clientes.ToListAsync();
        }

        // GET: api/Clientes/5
        // Obtiene un cliente específico por su ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(int id)
        {
            if (_context.Clientes == null)
            {
                return NotFound();
            }
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                return NotFound();
            }

            return cliente;
        }

        // PUT: api/Clientes/5
        // Actualiza un cliente existente
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(int id, Cliente cliente)
        {
            if (id != cliente.Id)
            {
                return BadRequest();
            }

            _context.Entry(cliente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClienteExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // 204 No Content para actualización exitosa
        }

        // POST: api/Clientes
        // Crea un nuevo cliente
        [HttpPost]
        public async Task<ActionResult<Cliente>> PostCliente(Cliente cliente)
        {
            if (_context.Clientes == null)
            {
                return Problem("Entity set 'ClientesDbContext.Clientes' is null.");
            }
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            // Retorna 201 CreatedAtAction con la URL del nuevo recurso creado
            return CreatedAtAction("GetCliente", new { id = cliente.Id }, cliente);
        }

        // DELETE: api/Clientes/5
        // Elimina un cliente por su ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            if (_context.Clientes == null)
            {
                return NotFound();
            }
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return NoContent(); // 204 No Content para eliminación exitosa
        }

        // Método auxiliar para verificar si un cliente existe
        private bool ClienteExists(int id)
        {
            return (_context.Clientes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}