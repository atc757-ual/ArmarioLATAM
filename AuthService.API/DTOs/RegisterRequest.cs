
// AuthService.API.Dtos/RegisterRequest.cs
using System.ComponentModel.DataAnnotations;

namespace AuthService.API.Dtos;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Name { get; set; } = default!;      // ✅ NUEVO
    public string BP { get; set; } = default!;  // ✅ NUEVO
}
