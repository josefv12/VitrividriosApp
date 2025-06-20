// VitrividriosApp.Web/SharedDtos/ClienteDto.cs
using System.ComponentModel.DataAnnotations; // Necesario para los atributos [Required], [StringLength], etc.

namespace VitrividriosApp.Web.SharedDtos
{
    // Este DTO representa cómo recibimos un Cliente desde ServicioClientes
    // y cómo lo mostramos/editamos en la UI.
    public class ClienteDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del cliente es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty; // Inicializado para evitar advertencias CS8618

        [StringLength(200, ErrorMessage = "La dirección no puede exceder 200 caracteres.")]
        public string Direccion { get; set; } = string.Empty; // Inicializado para evitar advertencias CS8618

        [StringLength(20, ErrorMessage = "El celular no puede exceder 20 caracteres.")]
        public string Celular { get; set; } = string.Empty; // Inicializado para evitar advertencias CS8618

        public bool EsMayorista { get; set; }
    }

    // DTO para enviar datos cuando creamos o actualizamos un cliente.
    // Incluimos el 'Id' para que el PageModel pueda usarlo tanto para crear (Id = 0)
    // como para actualizar (Id > 0). Las DataAnnotations son para la validación en la UI.
    public class CrearOActualizarClienteDto
    {
        public int Id { get; set; } // Incluimos el ID para la actualización, pero será 0 para crear

        [Required(ErrorMessage = "El nombre del cliente es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "La dirección no puede exceder 200 caracteres.")]
        public string Direccion { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "El celular no puede exceder 20 caracteres.")]
        public string Celular { get; set; } = string.Empty;

        public bool EsMayorista { get; set; }
    }
}