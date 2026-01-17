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




    public class KitDetailModel
    {
        public string Name { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Languages { get; set; } = string.Empty;
        public string UnitPrice { get; set; } = string.Empty;
        public string Total { get; set; } = string.Empty;

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
        public string Name { get; set; } = string.Empty;
        public string BP { get; set; } = string.Empty;
        public string Rol { get; init; } = string.Empty;
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
    public class ResultInfo
    {
        public string? Code { get; set; }
        public string? Description { get; set; }
    }
    public class KitType
    {
        public int KitTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string ImageURL { get; set; } = default!;
        public string ImageURLSelect { get; set; } = default!;
    }

    public class Garment
    {
        public int GarmentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int QuantityAuth { get; set; }

        // string crudo que llega del backend
        public string? Sizes { get; set; }

        // propiedad calculada para usar en la UI
        public List<string> SizeList =>
            string.IsNullOrWhiteSpace(Sizes)
                ? new List<string>()
                : Sizes.Split(',', StringSplitOptions.RemoveEmptyEntries)
                       .Select(s => s.Trim())
                       .ToList();

        public string? ImageURL { get; set; }
        public string? Languages { get; set; }
        public List<string> LanguageList =>
            string.IsNullOrWhiteSpace(Languages)
                ? new List<string>()
                : Languages.Split(',', StringSplitOptions.RemoveEmptyEntries)
                       .Select(s => s.Trim())
                       .ToList();

    }
    public class GarmentSelection
    {
        public int GarmentId { get; set; }
        public string? Name { get; set; }
        public int Quantity { get; set; }
        public string? Size { get; set; }
        public string? ImageURL { get; set; }
        public string? Languages { get; set; }
        public string? NativeLanguage { get; set; }
        public string? SecondLanguage{ get; set; }
        public string? ThirdLanguage { get; set; }
    
    }
    public class LanguageInfo
    {
        public string Code { get; set; } = string.Empty;   
        public string Name { get; set; } = string.Empty;   
        public string IconUrl { get; set; } = string.Empty; 
    }
    public class KitTypeSelection
    {
        public int KitTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

    }
    public class KitTypesResponse
    {
        public List<KitType> KitTypes { get; set; } = new();
        public int Count { get; set; }
        public ResultInfo? Result { get; set; }
    }
    public class GarmentsResponse
    {
        public List<Garment> Garments { get; set; } = new();
        public int Count { get; set; }
        public ResultInfo? Result { get; set; }
    }
    public class CreateOrderItem
    {
        public int GarmentId { get; set; }
        public string? Size { get; set; }
        public int Quantity { get; set; }
        public string? Languages { get; set; }
    }

    public class CreateOrder
    {
        public int KitTypeId { get; set; }
        public List<CreateOrderItem> Items { get; set; } = new();
        public string? Motive { get; set; }
        public string? DetailMotive { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Address { get; set; }
        public string? AddressReference { get; set; }
    }

    public class CreateOrderResponse
    {
        public string? Message { get; set; }
        public int OrderId { get; set; }
        public decimal Total { get; set; }
        public string? Status { get; set; }
        public string? Fecha { get; set; }
    }

    public class MyOrdersResponse
    {
        public int Count { get; set; }
        public List<MyOrderItem> Orders { get; set; } = new();
    }

    public class MyOrderItem
    {
        public int OrderId { get; set; }
        public string Fecha { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public string KitType { get; set; } = string.Empty;
        public string KitCode { get; set; } = string.Empty;
    }

    public class OrderDetailResponse
    {
        public int OrderId { get; set; }
        public string Fecha { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public string KitType { get; set; } = string.Empty;
        public string KitCode { get; set; } = string.Empty;
        public List<OrderDetailItem>? Items { get; set; }
    }

    public class OrderDetailItem
    {
        public string Garment { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
    public class PendingOrderResponse
    {
        public int Count { get; set; }
        public PendingOrderItem Order { get; set; } = new();
    }

    public class PendingOrderItem
    {
        public int OrderId { get; set; }
        public string Fecha { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public string KitType { get; set; } = string.Empty;
        public string KitCode { get; set; } = string.Empty;
    }

    public class DetailDelivered
    {
        public string Motive { get; set; } = string.Empty;
        public string DetailMotive { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
    }
    public class AuthSessionData
    {
        public string? Token { get; set; }
        public DateTime? TokenExpiration { get; set; }
        public string Name { get; set; } = string.Empty;
        public string BP { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }
    public class DataUserSession
    {
        public string Name { get; set; } = string.Empty;
        public string BP { get; set; } = string.Empty;
        public bool IsValid { get; set; }
        public string Rol { get; set; } = string.Empty;
    }

    public class ChangePassword
    {
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 50 caracteres")]
        public string Password { get; set; } = string.Empty;
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 50 caracteres")]
        [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
        public string PasswordConfirm { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }

    // Models/AdminUser.cs
    public class AdminModel
    {
        [Required(ErrorMessage = "El campo no puede estar vacío.")]
        public string BPSearch { get; set; } = string.Empty;
    }

    public class AdminUser
    {
        public string Name { get; set; } = string.Empty;
        public string Genero { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
    }
    public class PendingOrderDto
    {
        public bool check { get; set; }
        public int OrderId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string BP { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public string Kit { get; set; } = string.Empty;
    }

    public class OrderDetailItemDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string Languages {  get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Total { get; set; }
    }

}
