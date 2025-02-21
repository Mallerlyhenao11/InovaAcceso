using System.ComponentModel.DataAnnotations;

namespace InovaAcceso.Models
{
    public class Login
    {
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Clave { get; set; }

        [Display(Name = "Recuérdame")]
        public bool RememberMe { get; set; } // Campo para "Recuérdame"
    }
}