using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VitrividriosApp.Web.Data;
using VitrividriosApp.Web.Models;

namespace VitrividriosApp.Web.Pages.Clientes
{
    public class DetailsModel : PageModel
    {
        private readonly VitrividriosApp.Web.Data.ApplicationDbContext _context;

        public DetailsModel(VitrividriosApp.Web.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Cliente Cliente { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.FirstOrDefaultAsync(m => m.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }
            else
            {
                Cliente = cliente;
            }
            return Page();
        }
    }
}
