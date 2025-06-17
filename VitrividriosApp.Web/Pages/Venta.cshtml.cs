using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using VitrividriosApp.Web.Data;
using VitrividriosApp.Web.Models;

namespace VitrividriosApp.Web.Pages
{
    public class VentaModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public VentaModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<SelectListItem> Clientes { get; set; }
        public List<SelectListItem> Productos { get; set; }

        [BindProperty]
        public int SelectedClienteId { get; set; }
        [BindProperty]
        public int SelectedProductoId { get; set; }
        [BindProperty]
        public int Cantidad { get; set; } = 1;

        [TempData]
        public string CarritoJson { get; set; }
        public List<DetalleVenta> DetallesVenta { get; set; } = new List<DetalleVenta>();


        public async Task OnGetAsync()
        {
            await CargarDatosDesplegables();

            if (!string.IsNullOrEmpty(CarritoJson))
            {
                DetallesVenta = JsonSerializer.Deserialize<List<DetalleVenta>>(CarritoJson);
            }
        }

        public async Task<IActionResult> OnPostAnadirProductoAsync()
        {
            if (!string.IsNullOrEmpty(CarritoJson))
            {
                DetallesVenta = JsonSerializer.Deserialize<List<DetalleVenta>>(CarritoJson);
            }

            var productoAAgregar = await _context.Productos.FindAsync(SelectedProductoId);
            var cliente = await _context.Clientes.FindAsync(SelectedClienteId);

            if (productoAAgregar != null && cliente != null && productoAAgregar.Stock >= Cantidad)
            {
                decimal precioAplicado = cliente.EsMayorista
                                            ? productoAAgregar.PrecioMayorista
                                            : productoAAgregar.PrecioUnitario;

                DetallesVenta.Add(new DetalleVenta
                {
                    ProductoId = productoAAgregar.Id,
                    Producto = productoAAgregar,
                    Cantidad = Cantidad,
                    PrecioEnVenta = precioAplicado
                });
            }

            CarritoJson = JsonSerializer.Serialize(DetallesVenta);

            await CargarDatosDesplegables();
            return Page();
        }

        public async Task<IActionResult> OnPostEliminarProductoAsync(int index)
        {
            if (!string.IsNullOrEmpty(CarritoJson))
            {
                DetallesVenta = JsonSerializer.Deserialize<List<DetalleVenta>>(CarritoJson);
            }

            if (index >= 0 && index < DetallesVenta.Count)
            {
                DetallesVenta.RemoveAt(index);
            }

            CarritoJson = JsonSerializer.Serialize(DetallesVenta);

            await CargarDatosDesplegables();
            return Page();
        }

        // --- MÉTODO MODIFICADO: AHORA TIENE UN NOMBRE ESPECÍFICO ---
        public async Task<IActionResult> OnPostGuardarVentaAsync()
        {
            if (string.IsNullOrEmpty(CarritoJson))
            {
                await CargarDatosDesplegables();
                return Page();
            }
            DetallesVenta = JsonSerializer.Deserialize<List<DetalleVenta>>(CarritoJson);

            var venta = new Venta
            {
                ClienteId = SelectedClienteId,
                Fecha = DateTime.Now,
                Detalles = new List<DetalleVenta>()
            };

            decimal totalVenta = 0;
            foreach (var item in DetallesVenta)
            {
                var productoEnDB = await _context.Productos.FindAsync(item.ProductoId);
                if (productoEnDB == null || productoEnDB.Stock < item.Cantidad)
                {
                    await CargarDatosDesplegables();
                    return Page();
                }

                productoEnDB.Stock -= item.Cantidad;

                var detalle = new DetalleVenta
                {
                    ProductoId = item.ProductoId,
                    Cantidad = item.Cantidad,
                    PrecioEnVenta = item.PrecioEnVenta
                };
                venta.Detalles.Add(detalle);
                totalVenta += item.Cantidad * item.PrecioEnVenta;
            }

            venta.TotalVenta = totalVenta;

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();

            TempData.Remove("CarritoJson");

            return RedirectToPage("/Productos/Index");
        }

        private async Task CargarDatosDesplegables()
        {
            Clientes = await _context.Clientes
                               .Select(c => new SelectListItem
                               {
                                   Value = c.Id.ToString(),
                                   Text = c.Nombre
                               }).ToListAsync();

            Productos = await _context.Productos
                                .Where(p => p.Stock > 0)
                                .Select(p => new SelectListItem
                                {
                                    Value = p.Id.ToString(),
                                    Text = $"{p.Nombre} (Stock: {p.Stock})"
                                }).ToListAsync();
        }
    }
}