
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
    public string Name { get; set; } = default!;     
    public string BP { get; set; } = default!;
    public string Role { get; set; } = string.Empty;          
    public string Gender { get; set; } = string.Empty;    
    public string DNI { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }    
    public DateOnly ActivationDate { get; set; }
}
