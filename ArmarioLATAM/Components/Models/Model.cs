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

    public class AdminModel
    {
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string BPSearch { get; set; } = string.Empty;
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

    public class UsuarioModel
    {
        public string Nombre { get; set; } = "";
        public string Genero { get; set; } = "";
        public string Correo { get; set; } = "";
        public string Documento { get; set; } = "";
    }
    public class SolicitudModel
    {
        public bool check { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string BP { get; set; } = string.Empty;
        public string Orden { get; set; } = string.Empty;
        public string Precio { get; set; } = string.Empty;
        public string Kit { get; set; } = string.Empty;
    }

    public class KitDetailModel
    {
        public string Nombre { get; set; } = string.Empty;
        public string Talla { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public string PrecioUnitario { get; set; } = string.Empty;
        public string Total { get; set; } = string.Empty;

    }
    public class Garment
    {
        public int GarmentId { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public string? ImageUrl { get; set; }
        public int QuantityAuth { get; set; }
        public string? Sizes { get; set; }
        public string? Languages { get; set; }
        public List<string> SizesList =>

        string.IsNullOrWhiteSpace(Sizes)
            ? []
            : Sizes
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToList();
    }

    public class KitModel
    {
        public string Precio { get; set; } = string.Empty;
        public string KitName { get; set; } = string.Empty;
        public string Motivo { get; set; } = string.Empty;
    }
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
    }

    public class KitType
    {
        public int KitTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ImageURL { get; set; } = default!;
        public string ImageURLSelect { get; set; } = default!;
    }
}
