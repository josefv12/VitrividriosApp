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

        // ** AÑADE ESTA PROPIEDAD AQUÍ **
        // Esta propiedad calculará el TotalVenta para la vista.
        public decimal TotalVenta => DetallesVenta != null ? DetallesVenta.Sum(item => item.Cantidad * item.PrecioEnVenta) : 0m;


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
                    Producto = productoAAgregar, // Asegúrate de que este Producto sea el modelo completo si lo necesitas en la vista
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

        public async Task<IActionResult> OnPostGuardarVentaAsync()
        {
            if (string.IsNullOrEmpty(CarritoJson))
            {
                await CargarDatosDesplegables();
                return Page(); // No hay productos en el carrito, no guardar
            }
            DetallesVenta = JsonSerializer.Deserialize<List<DetalleVenta>>(CarritoJson);

            var venta = new Venta
            {
                ClienteId = SelectedClienteId,
                Fecha = DateTime.Now,
                Detalles = new List<DetalleVenta>()
            };

            decimal totalVentaCalculado = 0; // Usar un nombre diferente para evitar confusión con la propiedad TotalVenta del PageModel
            foreach (var item in DetallesVenta)
            {
                var productoEnDB = await _context.Productos.FindAsync(item.ProductoId);
                if (productoEnDB == null || productoEnDB.Stock < item.Cantidad)
                {
                    // Manejar error: producto no encontrado o stock insuficiente
                    // Podrías añadir un ModelState.AddModelError o un TempData para mostrar un mensaje al usuario
                    await CargarDatosDesplegables();
                    return Page();
                }

                productoEnDB.Stock -= item.Cantidad;

                var detalle = new DetalleVenta
                {
                    ProductoId = item.ProductoId,
                    Cantidad = item.Cantidad,
                    PrecioEnVenta = item.PrecioEnVenta // Este precio ya viene del carrito
                };
                venta.Detalles.Add(detalle);
                totalVentaCalculado += item.Cantidad * item.PrecioEnVenta;
            }

            venta.TotalVenta = totalVentaCalculado; // Asigna el total calculado al modelo de dominio Venta

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();

            TempData.Remove("CarritoJson"); // Limpiar el carrito después de guardar
            TempData["MensajeVentaExitosa"] = $"Venta registrada exitosamente con un total de {venta.TotalVenta.ToString("C")}.";

            // Redirige a la página de detalles de la venta recién creada
            return RedirectToPage("./Ventas/Details", new { id = venta.Id });
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