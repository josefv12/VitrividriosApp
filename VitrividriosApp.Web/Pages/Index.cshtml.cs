using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VitrividriosApp.Web.Pages
{
    public class IndexModel : PageModel
    {
        // Propiedades para capturar los mensajes de TempData.
        // Aunque la propiedad no se usa directamente en el code-behind
        // para l�gica compleja en este caso, es una buena pr�ctica
        // definirla si la vista Razor la va a acceder.
        [TempData]
        public string MensajeVentaExitosa { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        // El constructor b�sico. Si en el futuro necesitas inyectar servicios
        // (como ApplicationDbContext), los a�adir�as aqu�.
        public IndexModel()
        {
        }

        // El m�todo OnGet se ejecuta cuando se accede a la p�gina.
        // En este caso, no necesita l�gica compleja, ya que la vista
        // solo leer� los datos de TempData si est�n presentes.
        public void OnGet()
        {
            // No se requiere l�gica espec�fica aqu� para este dise�o,
            // pero es el lugar donde inicializar�as datos si la p�gina
            // principal necesitara cargar algo de la base de datos, por ejemplo.
        }
    }
}