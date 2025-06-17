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
    public class IndexModel : PageModel
    {
        private readonly VitrividriosApp.Web.Data.ApplicationDbContext _context;

        public IndexModel(VitrividriosApp.Web.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Cliente> Cliente { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Cliente = await _context.Clientes.ToListAsync();
        }
    }
}
