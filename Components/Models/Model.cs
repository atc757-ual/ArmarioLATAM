using System.ComponentModel.DataAnnotations;

namespace ArmarioLATAM.Components.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Ingresa un correo válido")]

        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 50 caracteres")]
        public string Contrasena { get; set; } = string.Empty;

        public bool Recordarme { get; set; }
    }

    public class LoginModelOnlyPassword
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Ingresa un correo válido")]
        public string Correo { get; set; } = string.Empty;
    }



    public class EmergencyModel
    {
        [Required(ErrorMessage = "Es obligatorio elegir un tipo de emergencia.")]
        public string EmergencyType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Es importante que describas él mótivo.")]
        public string DetalleMotivo { get; set; } = string.Empty;
    }

    public class AddressModel
    {
        [Required(ErrorMessage = "La dirección es obligatoria")]
         public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debes seleccionar una provincia.")]
        public string Province { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debes seleccionar un distrito.")]
        public string District { get; set; } = string.Empty;

        public string Reference { get; set; } = string.Empty;
    }

}
