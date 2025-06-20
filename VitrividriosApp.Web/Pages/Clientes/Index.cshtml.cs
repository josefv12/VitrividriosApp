using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq; // Para el método .Any()
using System.Net.Http;
using System.Net.Http.Json; // Para métodos ReadFromJsonAsync, PostAsJsonAsync, PutAsJsonAsync
using System.Threading.Tasks;
using VitrividriosApp.Web.SharedDtos; // Importa tus DTOs

namespace VitrividriosApp.Web.Pages.Clientes
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IList<ClienteDto> Clientes { get; set; } = new List<ClienteDto>();

        [BindProperty]
        public CrearOActualizarClienteDto InputCliente { get; set; } = new CrearOActualizarClienteDto();

        public async Task OnGetAsync()
        {
            await LoadClientes();
        }

        private async Task LoadClientes()
        {
            var httpClient = _httpClientFactory.CreateClient("ServicioClientes");
            try
            {
                Clientes = await httpClient.GetFromJsonAsync<List<ClienteDto>>("api/Clientes") ?? new List<ClienteDto>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al cargar clientes desde ServicioClientes: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Error al cargar los clientes. Por favor, asegúrese de que el ServicioClientes esté en ejecución.");
                Clientes = new List<ClienteDto>();
            }
        }

        public async Task<IActionResult> OnPostCreateOrUpdateAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadClientes();
                return Page();
            }

            var httpClient = _httpClientFactory.CreateClient("ServicioClientes");
            HttpResponseMessage response;

            if (InputCliente.Id == 0) // Crear nuevo cliente (POST)
            {
                response = await httpClient.PostAsJsonAsync("api/Clientes", InputCliente);
            }
            else // Actualizar cliente existente (PUT)
            {
                response = await httpClient.PutAsJsonAsync($"api/Clientes/{InputCliente.Id}", InputCliente);
            }

            if (response.IsSuccessStatusCode)
            {
                InputCliente = new CrearOActualizarClienteDto();
                await LoadClientes();
                return RedirectToPage("./Index");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error al guardar cliente: {response.StatusCode} - {errorContent}");
                await LoadClientes();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostEditAsync(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("ServicioClientes");
            var clienteToEdit = await httpClient.GetFromJsonAsync<ClienteDto>($"api/Clientes/{id}");
            if (clienteToEdit == null)
            {
                return NotFound();
            }

            InputCliente = new CrearOActualizarClienteDto
            {
                Id = clienteToEdit.Id,
                Nombre = clienteToEdit.Nombre,
                Direccion = clienteToEdit.Direccion,
                Celular = clienteToEdit.Celular,
                EsMayorista = clienteToEdit.EsMayorista
            };
            await LoadClientes();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("ServicioClientes");
            var response = await httpClient.DeleteAsync($"api/Clientes/{id}");

            if (response.IsSuccessStatusCode)
            {
                await LoadClientes();
                return RedirectToPage("./Index");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error al eliminar cliente: {response.StatusCode} - {errorContent}");
                await LoadClientes();
                return Page();
            }
        }
    }
}