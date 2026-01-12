using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthService.API.Data;
using AuthService.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.API.Services;

public class AuthService
{
    private readonly AuthDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(AuthDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    // =========================
    // REGISTRO DE USUARIOALmeria
    // =========================
    public async Task<User?> Register(string email, string password)
    {
        // Verificar si el usuario ya existe
        if (await _context.Users.AnyAsync(x => x.Email == email))
        {
            Console.WriteLine($"❌ Usuario ya existe: {email}");
            return null;
        }

        // Crear hash y salt
        CreatePasswordHash(password, out byte[] hash, out byte[] salt);

        var user = new User
        {
            Id = Guid.NewGuid(), // ✅ OBLIGATORIO (BD NO genera GUID)
            Email = email,
            PasswordHash = hash,
            PasswordSalt = salt,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        Console.WriteLine($"✅ Usuario registrado: {email}");
        Console.WriteLine($"🆔 UserId: {user.Id}");

        return user;
    }

    // =========================
    // LOGIN
    // =========================
    public async Task<string?> Login(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

        if (user == null)
        {
            Console.WriteLine("❌ Usuario no encontrado");
            return null;
        }

        if (!user.IsActive)
        {
            Console.WriteLine("❌ Usuario inactivo");
            return null;
        }

        var isValid = VerifyPassword(password, user.PasswordHash, user.PasswordSalt);

        if (!isValid)
        {
            Console.WriteLine("❌ Contraseña incorrecta");
            return null;
        }

        return GenerateJwt(user);
    }

    // =========================
    // PASSWORD HASHING
    // =========================
    private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
    {
        using var hmac = new HMACSHA512();
        salt = hmac.Key;
        hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    private bool VerifyPassword(string password, byte[] hash, byte[] salt)
    {
        using var hmac = new HMACSHA512(salt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(hash);
    }

    // =========================
    // JWT
    // =========================
    private string GenerateJwt(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
