// Archivo: VitrividriosApp.Web/Pages/Shared/Components/CreateClientModal/CreateClientModalViewComponent.cs
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using VitrividriosApp.Web.SharedDtos;
using System;

namespace VitrividriosApp.Web.Pages.Shared.Components.CreateClientModal
{
    // Esta clase DEBE terminar con 'ViewComponent' para que ASP.NET Core la encuentre automáticamente.
    public class CreateClientModalViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CreateClientModalViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // El método Invoke (o InvokeAsync) es el punto de entrada para renderizar el View Component.
        // Aquí no necesitamos pasar un modelo complejo, ya que la interacción es vía AJAX.
        public IViewComponentResult Invoke()
        {
            // Busca la vista por defecto "Default.cshtml" dentro de la misma carpeta
            // o en las ubicaciones convencionales de View Components.
            return View();
        }

        // Este handler se llamará desde JavaScript (Fetch API) para crear un nuevo cliente.
        // Es un método HTTP POST que recibe los datos del cliente del cuerpo de la solicitud JSON.
        // El framework automáticamente valida el 'nuevoCliente' debido a [FromBody] y llena ModelState.
        public async Task<IActionResult> OnPostCrearClienteAjax([FromBody] CrearOActualizarClienteDto nuevoCliente)
        {
            // Verificamos ModelState.IsValid directamente.
            // Si el DTO 'nuevoCliente' no es válido (por ejemplo, le faltan campos requeridos),
            // el ModelState ya contendrá esos errores.
            if (!ModelState.IsValid)
            {
                var errors = new Dictionary<string, List<string>>();
                foreach (var modelStateEntry in ModelState)
                {
                    if (modelStateEntry.Value.Errors.Any())
                    {
                        string key = modelStateEntry.Key;
                        List<string> errorMessages = modelStateEntry.Value.Errors.Select(e => e.ErrorMessage).ToList();

                        // Si la clave del error es la del modelo completo (vacía o similar)
                        if (string.IsNullOrEmpty(key) || key.Equals("nuevoCliente", StringComparison.OrdinalIgnoreCase))
                        {
                            // Agrega errores generales que no están atados a un campo específico
                            errors.Add("General", errorMessages);
                        }
                        else
                        {
                            // Para errores de campo específicos, la clave ya debe ser el nombre de la propiedad (ej. "Nombre")
                            // No es necesario reemplazar "nuevoCliente." aquí con [FromBody] en ViewComponent
                            errors.Add(key, errorMessages);
                        }
                    }
                }
                return new JsonResult(new { success = false, errors = errors });
            }

            var clientesHttpClient = _httpClientFactory.CreateClient("ServicioClientes");
            try
            {
                var response = await clientesHttpClient.PostAsJsonAsync("api/Clientes", nuevoCliente);

                if (response.IsSuccessStatusCode)
                {
                    var clienteCreado = await response.Content.ReadFromJsonAsync<ClienteDto>();
                    return new JsonResult(new { success = true, cliente = clienteCreado });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new JsonResult(new { success = false, error = $"Error del ServicioClientes: {response.StatusCode} - {errorContent}" });
                }
            }
            catch (HttpRequestException ex)
            {
                return new JsonResult(new { success = false, error = $"Error de comunicación con ServicioClientes: {ex.Message}" });
            }
        }
    }
}
