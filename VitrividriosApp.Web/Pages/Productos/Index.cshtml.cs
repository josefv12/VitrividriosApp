using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq; // Para el método .Any()
using System.Net.Http;
using System.Net.Http.Json; // Para métodos ReadFromJsonAsync, PostAsJsonAsync, PutAsJsonAsync
using System.Threading.Tasks;
using VitrividriosApp.Web.SharedDtos; // Importa tus DTOs

namespace VitrividriosApp.Web.Pages.Productos
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // Propiedad que contendrá la lista de productos para mostrar en la vista
        public IList<ProductoDto> Productos { get; set; } = new List<ProductoDto>();

        // Propiedad para enlazar los datos del formulario de creación/edición
        [BindProperty]
        public CrearOActualizarProductoDto InputProducto { get; set; } = new CrearOActualizarProductoDto();

        // Método que se ejecuta al cargar la página (GET request)
        public async Task OnGetAsync()
        {
            await LoadProductos();
        }

        // Método privado para cargar los productos desde el ServicioCatalogo
        private async Task LoadProductos()
        {
            var httpClient = _httpClientFactory.CreateClient("ServicioCatalogo");
            try
            {
                // Llama a la API GET /api/Productos para obtener todos los productos
                Productos = await httpClient.GetFromJsonAsync<List<ProductoDto>>("api/Productos") ?? new List<ProductoDto>();
            }
            catch (HttpRequestException ex)
            {
                // Manejo de errores básico para la carga inicial:
                Console.WriteLine($"Error al cargar productos desde ServicioCatalogo: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Error al cargar los productos. Por favor, asegúrese de que el ServicioCatalogo esté en ejecución.");
                Productos = new List<ProductoDto>(); // Asegurar que la lista no sea null incluso con error
            }
        }

        // Manejador para la acción POST de crear o actualizar un producto
        public async Task<IActionResult> OnPostCreateOrUpdateAsync()
        {
            // Verifica la validación del modelo (DataAnnotations en CrearOActualizarProductoDto)
            if (!ModelState.IsValid)
            {
                await LoadProductos(); // Recargar productos para que la tabla se vea bien si hay errores
                return Page(); // Vuelve a mostrar la página con los errores de validación
            }

            var httpClient = _httpClientFactory.CreateClient("ServicioCatalogo");
            HttpResponseMessage response;

            if (InputProducto.Id == 0) // Si el ID es 0, es un nuevo producto (POST)
            {
                response = await httpClient.PostAsJsonAsync("api/Productos", InputProducto);
            }
            else // Si el ID no es 0, es una actualización de un producto existente (PUT)
            {
                response = await httpClient.PutAsJsonAsync($"api/Productos/{InputProducto.Id}", InputProducto);
            }

            if (response.IsSuccessStatusCode)
            {
                InputProducto = new CrearOActualizarProductoDto(); // Limpiar el formulario
                await LoadProductos(); // Recargar la lista de productos
                return RedirectToPage("./Index"); // Redirigir para limpiar el estado del formulario y la URL
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error al guardar producto: {response.StatusCode} - {errorContent}");
                await LoadProductos(); // Recargar productos para que la tabla se vea bien con errores
                return Page();
            }
        }

        // Manejador para la acción POST de editar un producto (cuando se hace clic en "Editar")
        // Usamos SupportsGet = true si queremos que esta acción pueda ser llamada con GET (ej. desde un link),
        // pero con un formulario POST es más común usar un input hidden para el id.
        public async Task<IActionResult> OnPostEditAsync(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("ServicioCatalogo");
            var productoToEdit = await httpClient.GetFromJsonAsync<ProductoDto>($"api/Productos/{id}");
            if (productoToEdit == null)
            {
                return NotFound(); // Retorna 404 si el producto no se encuentra
            }

            // Cargar los datos del producto en el formulario de entrada
            InputProducto = new CrearOActualizarProductoDto
            {
                Id = productoToEdit.Id, // Asegurarse de que el ID esté en el InputProducto
                Nombre = productoToEdit.Nombre,
                Descripcion = productoToEdit.Descripcion,
                PrecioUnitario = productoToEdit.PrecioUnitario,
                PrecioMayorista = productoToEdit.PrecioMayorista,
                Stock = productoToEdit.Stock
            };
            await LoadProductos(); // Recargar la lista para mostrarla actualizada
            return Page(); // Mantenerse en la misma página para mostrar el formulario pre-llenado
        }

        // Manejador para la acción POST de eliminar un producto
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("ServicioCatalogo");
            // Llama a la API DELETE /api/Productos/{id}
            var response = await httpClient.DeleteAsync($"api/Productos/{id}");

            if (response.IsSuccessStatusCode)
            {
                await LoadProductos(); // Recargar la lista de productos
                return RedirectToPage("./Index"); // Redirigir para limpiar el estado
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error al eliminar producto: {response.StatusCode} - {errorContent}");
                await LoadProductos(); // Recargar productos para que la tabla se vea bien con errores
                return Page();
            }
        }
    }
}