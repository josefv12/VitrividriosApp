using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VitrividriosApp.Web.SharedDtos;
using System;

namespace VitrividriosApp.Web.Pages.Ventas
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Propiedades para mostrar en la vista
        public IList<VentaDto> Ventas { get; set; } = new List<VentaDto>();
        public IList<SelectListItem> ClientesDisponibles { get; set; } = new List<SelectListItem>();
        public IList<SelectListItem> ProductosDisponibles { get; set; } = new List<SelectListItem>();

        // Propiedad para enlazar los datos del formulario de creación de venta
        [BindProperty]
        public CrearVentaRequestDto InputVenta { get; set; } = new CrearVentaRequestDto();

        // Propiedades auxiliares para el formulario de agregar ítems
        [BindProperty]
        public int SelectedProductoId { get; set; }
        [BindProperty]
        public int SelectedCantidad { get; set; } = 1; // Cantidad por defecto

        // Ya no hay propiedades ni métodos relacionados con la creación del cliente aquí.
        // Toda esa lógica se movió a Pages/Shared/Components/CreateClientModal/CreateClientModal.cshtml.cs

        public async Task OnGetAsync()
        {
            await LoadDataForPage();
        }

        private async Task LoadDataForPage()
        {
            var ventasHttpClient = _httpClientFactory.CreateClient("ServicioVentas");
            var clientesHttpClient = _httpClientFactory.CreateClient("ServicioClientes");
            var catalogoHttpClient = _httpClientFactory.CreateClient("ServicioCatalogo");

            try
            {
                var ventas = await ventasHttpClient.GetFromJsonAsync<List<VentaDto>>("api/Ventas") ?? new List<VentaDto>();
                foreach (var venta in ventas)
                {
                    try
                    {
                        var cliente = await clientesHttpClient.GetFromJsonAsync<ClienteDto>($"api/Clientes/{venta.ClienteId}");
                        if (cliente != null)
                        {
                            venta.ClienteNombre = cliente.Nombre;
                        }
                    }
                    catch (HttpRequestException) { /* Ignorar si no se encuentra cliente */ }

                    foreach (var detalle in venta.Detalles)
                    {
                        try
                        {
                            var producto = await catalogoHttpClient.GetFromJsonAsync<ProductoDto>($"api/Productos/{detalle.ProductoId}");
                            if (producto != null)
                            {
                                detalle.ProductoNombre = producto.Nombre;
                            }
                        }
                        catch (HttpRequestException) { /* Ignorar o manejar */ }
                    }
                }
                Ventas = ventas;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al cargar ventas desde ServicioVentas: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Error al cargar las ventas. Asegúrese de que el ServicioVentas esté en ejecución.");
            }

            try
            {
                var clientes = await clientesHttpClient.GetFromJsonAsync<List<ClienteDto>>("api/Clientes") ?? new List<ClienteDto>();
                ClientesDisponibles = clientes.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nombre }).ToList();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al cargar clientes para el desplegable: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Error al cargar los clientes disponibles.");
            }

            try
            {
                var productos = await catalogoHttpClient.GetFromJsonAsync<List<ProductoDto>>("api/Productos") ?? new List<ProductoDto>();
                ProductosDisponibles = productos.Select(p => new SelectListItem { Value = p.Id.ToString(), Text = $"{p.Nombre} (Stock: {p.Stock})" }).ToList();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al cargar productos para el desplegable: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Error al cargar los productos disponibles.");
            }
        }

        public async Task<IActionResult> OnPostAddItemAsync()
        {
            if (SelectedProductoId > 0 && SelectedCantidad > 0)
            {
                InputVenta.Items.Add(new ItemVentaRequestDto
                {
                    ProductoId = SelectedProductoId,
                    Cantidad = SelectedCantidad
                });
                SelectedProductoId = 0;
                SelectedCantidad = 1;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Por favor, seleccione un producto y una cantidad válida para agregar.");
            }
            await LoadDataForPage();
            return Page();
        }

        public async Task<IActionResult> OnPostRemoveItemAsync(int index)
        {
            if (index >= 0 && index < InputVenta.Items.Count)
            {
                InputVenta.Items.RemoveAt(index);
            }
            await LoadDataForPage();
            return Page();
        }

        public async Task<IActionResult> OnPostCreateVentaAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadDataForPage();
                return Page();
            }

            if (!InputVenta.Items.Any())
            {
                ModelState.AddModelError(string.Empty, "La venta debe contener al menos un producto.");
                await LoadDataForPage();
                return Page();
            }

            var ventasHttpClient = _httpClientFactory.CreateClient("ServicioVentas");
            try
            {
                var response = await ventasHttpClient.PostAsJsonAsync("api/Ventas", InputVenta);

                if (response.IsSuccessStatusCode)
                {
                    InputVenta = new CrearVentaRequestDto();
                    return RedirectToPage("./Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Error al crear la venta: {response.StatusCode} - {errorContent}");
                    await LoadDataForPage();
                    return Page();
                }
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, $"Error de comunicación con ServicioVentas: {ex.Message}");
                await LoadDataForPage();
                return Page();
            }
        }
    }
}