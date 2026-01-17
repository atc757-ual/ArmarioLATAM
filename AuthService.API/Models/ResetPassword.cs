using System.ComponentModel.DataAnnotations;

namespace AuthService.API.Models;
public class ResetPassword
{
    public int Id { get; set; }
    public Guid UserId { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool Used { get; set; }

    public User User { get; set; } = null!;
}
