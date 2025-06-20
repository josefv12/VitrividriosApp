
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VitrividriosApp.Web.SharedDtos; // Asegúrate de que este namespace y el DTO existan

namespace VitrividriosApp.Web.Pages.Clientes
{
    public class CreateModel : PageModel
    {
        // 1. Cambiamos el DbContext por el IHttpClientFactory
        private readonly IHttpClientFactory _httpClientFactory;

        public CreateModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public CrearOActualizarClienteDto Cliente { get; set; } = new();

        public IActionResult OnGet()
        {
            return Page();
        }

        // 2. Cambiamos la lógica para que llame a la API en lugar de usar el DbContext
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Creamos un cliente HTTP para hablar con el ServicioClientes
            var httpClient = _httpClientFactory.CreateClient("ServicioClientes");

            try
            {
                // Llamamos a la API del microservicio para crear el cliente
                var response = await httpClient.PostAsJsonAsync("api/Clientes", Cliente);

                if (response.IsSuccessStatusCode)
                {
                    // Si la creación fue exitosa, redirigir a la página de Ventas como planeamos
                    return RedirectToPage("/Ventas/Index");
                }
                else
                {
                    // Si la API devuelve un error, lo mostramos en la página
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Error del servicio: {errorContent}");
                    return Page();
                }
            }
            catch (HttpRequestException ex)
            {
                // Si hay un error de conexión, lo mostramos en la página
                ModelState.AddModelError(string.Empty, $"Error de red: No se pudo conectar al servicio. {ex.Message}");
                return Page();
            }
        }
    }
}