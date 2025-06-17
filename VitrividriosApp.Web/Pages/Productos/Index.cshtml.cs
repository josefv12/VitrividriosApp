using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using VitrividriosApp.Web.Data;
using VitrividriosApp.Web.Models;

namespace VitrividriosApp.Web.Pages.Productos
{
    public class IndexModel : PageModel
    {
        private readonly VitrividriosApp.Web.Data.ApplicationDbContext _context;

        public IndexModel(VitrividriosApp.Web.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Producto> Producto { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Producto = await _context.Productos.ToListAsync();
        }
    }
}
