using System.ComponentModel.DataAnnotations;

namespace AuthService.API.Models;

public class User
{
    [Key]
    public Guid Id { get; set; }   // GUID manual

    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = default!;

    [Required]
    public byte[] PasswordHash { get; set; } = default!;

    [Required]
    public byte[] PasswordSalt { get; set; } = default!;

    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
