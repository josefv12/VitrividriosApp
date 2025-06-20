using System.ComponentModel.DataAnnotations;

namespace ServicioClientes.Models // Asegúrate de que el namespace sea ServicioClientes.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del cliente es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public required string Nombre { get; set; } // Modificador 'required' añadido

        [StringLength(200, ErrorMessage = "La dirección no puede exceder los 200 caracteres.")]
        public required string Direccion { get; set; } // Modificador 'required' añadido

        [StringLength(20, ErrorMessage = "El celular no puede exceder los 20 caracteres.")]
        public required string Celular { get; set; } // Modificador 'required' añadido

        public bool EsMayorista { get; set; }
    }
}