
// AuthService.API.Dtos/RegisterRequest.cs
namespace AuthService.API.Dtos;

public class RegisterRequest
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Name { get; set; } = default!;      // ✅ NUEVO
    public string BP { get; set; } = default!;  // ✅ NUEVO
}
