using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VitrividriosApp.Web.Data;
using VitrividriosApp.Web.Models; // Asegúrate de que este namespace sea correcto para tus modelos

namespace VitrividriosApp.Web.Pages.Ventas
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Propiedad para almacenar la venta que se va a mostrar
        public Venta Venta { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                // Si no se proporciona un ID, redirige al historial de ventas o muestra un error
                return NotFound(); // O RedirectToPage("./Index");
            }

            // Carga la venta incluyendo el cliente y todos los detalles de venta,
            // y para cada detalle, carga el producto asociado.
            Venta = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Venta == null)
            {
                // Si la venta no se encuentra, devuelve un error 404
                return NotFound();
            }
            return Page();
        }
    }
}