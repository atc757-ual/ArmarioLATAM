// AuthService.API.Models/User.cs
using System.ComponentModel.DataAnnotations;

namespace AuthService.API.Models;

public class User
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = default!;
    [Required]
    public byte[] PasswordHash { get; set; } = default!;
    [Required]
    public byte[] PasswordSalt { get; set; } = default!;
    [Required]
    [MaxLength(250)]
    public string Name { get; set; } = default!;  
    [Required]
    [MaxLength(10)]
    public string BP { get; set; } = default!;  
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Role { get; set; } = default!;      
    public string Gender { get; set; } = default!;    
    public string DNI { get; set; } = default!;
    public DateOnly BirthDate { get; set; }
    public DateOnly ActivationDate { get; set; } 

}