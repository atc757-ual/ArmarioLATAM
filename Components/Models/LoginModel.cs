using System.ComponentModel.DataAnnotations;

namespace ArmarioLATAM.Components.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Ingresa un correo válido")]
        public string? Correo { get; set; } 

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(50, MinimumLength = 8,
            ErrorMessage = "La contraseña debe tener entre 8 y 50 caracteres")]
        public string? Contrasena { get; set; } 

        public bool Recordarme { get; set; }
    }
}
