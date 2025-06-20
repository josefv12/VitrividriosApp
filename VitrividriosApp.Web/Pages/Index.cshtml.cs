using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VitrividriosApp.Web.Pages
{
    public class IndexModel : PageModel
    {
        // Propiedades para capturar los mensajes de TempData.
        // Aunque la propiedad no se usa directamente en el code-behind
        // para lógica compleja en este caso, es una buena práctica
        // definirla si la vista Razor la va a acceder.
        [TempData]
        public string MensajeVentaExitosa { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        // El constructor básico. Si en el futuro necesitas inyectar servicios
        // (como ApplicationDbContext), los añadirías aquí.
        public IndexModel()
        {
        }

        // El método OnGet se ejecuta cuando se accede a la página.
        // En este caso, no necesita lógica compleja, ya que la vista
        // solo leerá los datos de TempData si están presentes.
        public void OnGet()
        {
            // No se requiere lógica específica aquí para este diseño,
            // pero es el lugar donde inicializarías datos si la página
            // principal necesitara cargar algo de la base de datos, por ejemplo.
        }
    }
}