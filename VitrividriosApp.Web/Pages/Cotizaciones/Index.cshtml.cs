// Archivo: VitrividriosApp.Web/Pages/Cotizaciones/Index.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VitrividriosApp.Web.SharedDtos; // Para los DTOs de Cliente, Producto, Cotizacion
using System;

namespace VitrividriosApp.Web.Pages.Cotizaciones
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Propiedades para mostrar en la vista
        public IList<CotizacionDto> Cotizaciones { get; set; } = new List<CotizacionDto>(); // Para el historial de cotizaciones
        public IList<SelectListItem> ClientesDisponibles { get; set; } = new List<SelectListItem>();
        public IList<SelectListItem> ProductosDisponibles { get; set; } = new List<SelectListItem>();

        // Propiedad para enlazar los datos del formulario de creación de cotización
        [BindProperty]
        public CrearCotizacionRequestDto InputCotizacion { get; set; } = new CrearCotizacionRequestDto();

        // Propiedades auxiliares para el formulario de agregar ítems
        [BindProperty]
        public int SelectedProductoId { get; set; }
        [BindProperty]
        public int SelectedCantidad { get; set; } = 1; // Cantidad por defecto

        // Propiedad calculada para el total de la cotización actual en el carrito
        public decimal TotalCarritoCotizacion => InputCotizacion.Items.Sum(item => item.Cantidad * item.PrecioEnCotizacion);


        public async Task OnGetAsync()
        {
            await LoadDataForPage();
        }

        private async Task LoadDataForPage()
        {
            var cotizacionesHttpClient = _httpClientFactory.CreateClient("ServicioVentas"); // Reutilizamos ServicioVentas para cotizaciones
            var clientesHttpClient = _httpClientFactory.CreateClient("ServicioClientes");
            var catalogoHttpClient = _httpClientFactory.CreateClient("ServicioCatalogo");

            try
            {
                // Cargar el historial de cotizaciones
                // La ruta para cotizaciones será api/Cotizaciones en el ServicioVentas
                var cotizaciones = await cotizacionesHttpClient.GetFromJsonAsync<List<CotizacionDto>>("api/Cotizaciones") ?? new List<CotizacionDto>();
                foreach (var cotizacion in cotizaciones)
                {
                    try
                    {
                        var cliente = await clientesHttpClient.GetFromJsonAsync<ClienteDto>($"api/Clientes/{cotizacion.ClienteId}");
                        if (cliente != null)
                        {
                            cotizacion.ClienteNombre = cliente.Nombre;
                        }
                    }
                    catch (HttpRequestException) { /* Ignorar si no se encuentra cliente */ }

                    foreach (var detalle in cotizacion.Detalles)
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
                Cotizaciones = cotizaciones;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al cargar cotizaciones desde ServicioVentas: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Error al cargar las cotizaciones. Asegúrese de que el ServicioVentas esté en ejecución y los endpoints de cotización estén disponibles.");
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
                var clientesHttpClient = _httpClientFactory.CreateClient("ServicioClientes");
                var catalogoHttpClient = _httpClientFactory.CreateClient("ServicioCatalogo");

                ClienteDto cliente = null;
                try
                {
                    if (InputCotizacion.ClienteId > 0)
                    {
                        cliente = await clientesHttpClient.GetFromJsonAsync<ClienteDto>($"api/Clientes/{InputCotizacion.ClienteId}");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Debe seleccionar un cliente antes de añadir productos a la cotización.");
                        await LoadDataForPage();
                        return Page();
                    }
                }
                catch (HttpRequestException ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error al obtener datos del cliente: {ex.Message}");
                    await LoadDataForPage();
                    return Page();
                }

                ProductoDto producto = null;
                try
                {
                    producto = await catalogoHttpClient.GetFromJsonAsync<ProductoDto>($"api/Productos/{SelectedProductoId}");
                }
                catch (HttpRequestException ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error al obtener datos del producto: {ex.Message}");
                    await LoadDataForPage();
                    return Page();
                }

                if (cliente != null && producto != null) // Para cotizaciones, no validamos stock aquí a menos que sea una regla de negocio
                {
                    decimal precioAplicado = cliente.EsMayorista
                                             ? producto.PrecioMayorista
                                             : producto.PrecioUnitario;

                    InputCotizacion.Items.Add(new ItemCotizacionRequestDto
                    {
                        ProductoId = SelectedProductoId,
                        Cantidad = SelectedCantidad,
                        PrecioEnCotizacion = precioAplicado // Guardamos el precio en la cotización
                    });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al añadir producto a la cotización. Verifique selección de cliente/producto.");
                }

                SelectedProductoId = 0;
                SelectedCantidad = 1;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Por favor, seleccione un producto y una cantidad válida para agregar a la cotización.");
            }
            await LoadDataForPage();
            return Page();
        }

        public async Task<IActionResult> OnPostRemoveItemAsync(int index)
        {
            if (index >= 0 && index < InputCotizacion.Items.Count)
            {
                InputCotizacion.Items.RemoveAt(index);
            }
            await LoadDataForPage();
            return Page();
        }

        public async Task<IActionResult> OnPostCreateCotizacionAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadDataForPage();
                return Page();
            }

            if (!InputCotizacion.Items.Any())
            {
                ModelState.AddModelError(string.Empty, "La cotización debe contener al menos un producto.");
                await LoadDataForPage();
                return Page();
            }

            // El TotalCotizacion se calculará en el ServicioVentas, pero lo podríamos calcular aquí también para un display inmediato.

            var cotizacionesHttpClient = _httpClientFactory.CreateClient("ServicioVentas");
            try
            {
                // La ruta será api/Cotizaciones en el ServicioVentas
                var response = await cotizacionesHttpClient.PostAsJsonAsync("api/Cotizaciones", InputCotizacion);

                if (response.IsSuccessStatusCode)
                {
                    InputCotizacion = new CrearCotizacionRequestDto(); // Limpiar el formulario
                    return RedirectToPage("./Index"); // Redirigir a la misma página para ver la lista actualizada
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Error al crear la cotización: {response.StatusCode} - {errorContent}");
                    await LoadDataForPage();
                    return Page();
                }
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, $"Error de comunicación con ServicioVentas para cotizaciones: {ex.Message}");
                await LoadDataForPage();
                return Page();
            }
        }
    }
}