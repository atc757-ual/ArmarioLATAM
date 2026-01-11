// AuthService.API.Services/AuthService.cs - AÑADE ESTOS MÉTODOS:
using System.Security.Cryptography;
using System.Text;
using AuthService.API.Data;
using AuthService.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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

    // MÉTODO PARA REGISTRAR NUEVO USUARIO
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
    // Id se asignará automáticamente por la BD (auto-incremental)
    Email = email,
    PasswordHash = hash,
    PasswordSalt = salt,
    IsActive = true,
    CreatedAt = DateTime.UtcNow
};
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        Console.WriteLine($"✅ Usuario registrado: {email}");
        Console.WriteLine($"🔐 Hash generado: {BitConverter.ToString(hash)}");
        Console.WriteLine($"🧂 Salt generado: {BitConverter.ToString(salt)}");

        return user;
    }

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

        Console.WriteLine($"✅ Usuario encontrado: {user.Email}");
        Console.WriteLine($"🔐 Hash almacenado: {BitConverter.ToString(user.PasswordHash)}");
        Console.WriteLine($"🧂 Salt almacenado: {BitConverter.ToString(user.PasswordSalt)}");

        var isValid = VerifyPassword(password, user.PasswordHash, user.PasswordSalt);
        Console.WriteLine($"🔓 Contraseña válida: {isValid}");

        if (!isValid)
        {
            // Para debug: calcular hash con el salt de la BD
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            Console.WriteLine($"🔑 Hash calculado: {BitConverter.ToString(computed)}");
            Console.WriteLine($"📏 Longitud hash BD: {user.PasswordHash.Length}");
            Console.WriteLine($"📏 Longitud hash calc: {computed.Length}");
            return null;
        }

        return GenerateJwt(user.Email);
    }

    // MÉTODO PARA CREAR HASH (USADO EN REGISTRO)
    private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
    {
        using var hmac = new HMACSHA512();
        salt = hmac.Key;
        hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    private bool VerifyPassword(string password, byte[] hash, byte[] salt)
    {
        using var hmac = new HMACSHA512(salt);
        var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return computed.SequenceEqual(hash);
    }

    private string GenerateJwt(string email)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
        );
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: new[]
            {
                new Claim(ClaimTypes.Email, email)
            },
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}