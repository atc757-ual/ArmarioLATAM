using AuthService.API.Data;
using AuthService.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static System.Net.WebRequestMethods;
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
    // REGISTRO DE USUARIO
    // =========================
    // AuthService.API.Services/AuthService.cs
    public async Task<User?> Register(string email, string password, string name, string passport)
    {
        if (await _context.Users.AnyAsync(x => x.Email == email))
        {
            Console.WriteLine($"❌ Usuario ya existe: {email}");
            return null;
        }

        CreatePasswordHash(password, out byte[] hash, out byte[] salt);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = hash,
            PasswordSalt = salt,
            Name = name,          // ✅ NUEVO
            BP = passport,  // ✅ NUEVO
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        Console.WriteLine($"✅ Usuario registrado: {email}");
        Console.WriteLine($"🆔 UserId: {user.Id}");

        return user;
    }

    // Modificar el método Login para retornar el User completo
    public async Task<User?> LoginAndGetUser(string email, string password)
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
    public string GenerateJwt(User user)
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
    public async Task<bool> ResetPasswordAsync(string token, string newPassword)
    {
        var request = await _context.ResetPassword
            .Include(r => r.User)
            .FirstOrDefaultAsync(r =>
                r.Token == token &&
                !r.Used &&
                r.ExpiresAt > DateTime.UtcNow);

        if (request == null)
            return false;

        CreatePasswordHash(newPassword, out byte[] hash, out byte[] salt);
        request.User.PasswordHash = hash;
        request.User.PasswordSalt = salt;
        request.Used = true;

        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> SendPasswordResetAsync(string email)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

        if (user == null)
            return false;

        var token = Guid.NewGuid().ToString("N");
        var expires = DateTime.UtcNow.AddHours(1);

        var request = new ResetPassword
        {
            UserId = user.Id,
            Email = user.Email,
            Token = token,
            ExpiresAt = expires,
            Used = false
        };

        // OJO: el nombre debe coincidir con tu DbSet en AuthDbContext
        _context.ResetPassword.Add(request);
        await _context.SaveChangesAsync();

        var frontendBaseUrl = _config["Frontend:BaseUrl"] ?? "https://localhost:7177";
        var resetLink = $"{frontendBaseUrl}/ResetPassword?token={token}";

        var fechaLimite = expires.ToString("dd/MM/yyyy HH:mm");
        var nombreCompleto = user.Name ?? string.Empty;

        string firstName;

        var partes = nombreCompleto
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        firstName = partes.Length > 0 ? partes[0] : string.Empty;
    
        var body = $@"
            <!DOCTYPE html>
            <html lang=""es"">
            <head>
                <meta charset=""UTF-8"">
                <title>Recuperar contraseña - ArmarioLATAM</title>
            </head>
            <body style=""margin:0;padding:0;font-family:Arial,Helvetica,sans-serif;background-color:#f5f5f5;"">
                <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#f5f5f5;padding:24px 0;"">
                    <tr>
                        <td align=""center"">
                            <table width=""640"" cellpadding=""0"" cellspacing=""0""
                                   style=""background-color:#ffffff;border-radius:16px;
                                          box-shadow:0 4px 18px rgba(0,0,0,0.08);overflow:hidden;"">
                                <!-- Header -->
                                <tr>
                                    <td style=""background-color:#10004F;
                                               padding:24px 32px;color:#ffffff;font-size:20px;
                                               font-weight:bold;"">
                                        ArmarioLATAM
                                    </td>

                                </tr>

                                <!-- Content -->
                                <tr>
                                    <td style=""padding:28px 32px;color:#111827;font-size:15px;line-height:1.7;"">
                                        <p style=""margin:0 0 14px 0;"">¡Hola, <strong>{firstName}!</strong></p>

                                        <p style=""margin:0 0 14px 0;"">
                                            Hemos recibido una solicitud para <strong>restablecer tu contraseña</strong>.
                                        </p>

                                        <p style=""margin:0 0 18px 0;"">
                                            Para continuar, haz clic en el botón de abajo y sigue las instrucciones
                                            en la página de ArmarioLATAM.
                                        </p>

                                        <!-- Botón -->
                                        <p style=""margin:20px 0;text-align:center;"">
                                            <a href=""{resetLink}""
                                               style=""display:inline-block;padding:14px 28px;border-radius:999px;
                                                      background-color:#10004F;color:#ffffff;text-decoration:none;
                                                      font-weight:bold;font-size:15px;"">
                                                Cambiar mi contraseña
                                            </a>
                                        </p>

                                        <!-- URL enmarcada -->
                                        <p style=""margin:22px 0 8px 0;font-size:12px;color:#6b7280;"">
                                            Si el botón no funciona, copia y pega este enlace en tu navegador:
                                        </p>

                                        <p style=""margin:0 0 16px 0;font-size:12px;color:#4b5563;
                                                  padding:10px 14px;border-radius:10px;
                                                  background-color:#f3f4f6;word-break:break-all;"">
                                            {resetLink}
                                        </p>

                                        <p style=""margin:4px 0 8px 0;font-size:12px;color:#6b7280;"">
                                            Este enlace será válido hasta <strong>{fechaLimite}</strong>.
                                            Después de ese momento, deberás solicitar un nuevo enlace de recuperación.
                                        </p>

                                        <p style=""margin:10px 0 0 0;font-size:12px;color:#9ca3af;"">
                                            Si no fuiste tú quien solicitó cambiar la contraseña,
                                            puedes ignorar este correo; tu cuenta seguirá siendo segura.
                                        </p>
                                    </td>
                                </tr>

                                <!-- Footer -->
                                <tr>
                                    <td style=""padding:18px 32px;background-color:#f9fafb;
                                               color:#9ca3af;font-size:11px;text-align:center;"">
                                        © {DateTime.UtcNow.ToString("yyyy")} ArmarioLATAM. Todos los derechos reservados.
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </body>
            </html>";

        await SendEmailAsync(user.Email, "Recuperar contraseña - ArmarioLATAM", body);

        return true;
    }
    private async Task SendEmailAsync(string to, string subject, string bodyHtml)
    {
        var server = _config["EmailSettings:Server"];
        var port = int.Parse(_config["EmailSettings:Port"] ?? "587");
        var senderEmail = _config["EmailSettings:SenderEmail"];
        var senderName = _config["EmailSettings:SenderName"];
        var password = _config["EmailSettings:Password"]; 

        using var message = new MailMessage();
        message.From = new MailAddress(senderEmail!, senderName);
        message.To.Add(to);
        message.Subject = subject;
        message.Body = bodyHtml;
        message.IsBodyHtml = true;

        using var client = new SmtpClient(server!, port);
        client.EnableSsl = true;
        client.UseDefaultCredentials = false;
        client.Credentials = new NetworkCredential(senderEmail, password);

        await client.SendMailAsync(message);
    }

}