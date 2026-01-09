// AuthService.API.Models/User.cs
using System.ComponentModel.DataAnnotations;

namespace AuthService.API.Models;

public class User
{
    public Guid Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    public byte[] PasswordHash { get; set; } = default!;
    public byte[] PasswordSalt { get; set; } = default!;

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}