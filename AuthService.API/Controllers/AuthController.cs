// AuthService.API.Controllers/AuthController.cs
using AuthService.API.Dtos;
using AuthService.API.Services;
using Microsoft.AspNetCore.Mvc;
using AuthServiceClass = AuthService.API.Services.AuthService;

namespace AuthService.API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly AuthServiceClass _auth;

    public AuthController(AuthServiceClass auth)
    {
        _auth = auth;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        try
        {
            var user = await _auth.Register(request.Email, request.Password, request.Name, request.BP);

            if (user == null)
                return BadRequest(new
                {
                    message = "Usuario ya existe o datos inválidos",
                    Result = new
                    {
                        Code = "400",
                        Description = "Bad Request"
                    }
                });

            return Ok(new
            {
                message = "Usuario registrado exitosamente",
                userId = user.Id,
                email = user.Email,
                name = user.Name,        // ✅ NUEVO
                bp = user.BP, // ✅ NUEVO
                Result = new
                {
                    Code = "200",
                    Description = "OK"
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error interno del servidor",
                error = ex.Message,
                Result = new
                {
                    Code = "500",
                    Description = "Internal Server Error"
                }
            });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        try
        {
            var user = await _auth.LoginAndGetUser(request.Email, request.Password);

            if (user == null)
                return Unauthorized(new
                {
                    message = "Credenciales inválidas",
                    Result = new
                    {
                        Code = "401",
                        Description = "Unauthorized"
                    }
                });

            var token = _auth.GenerateJwt(user); // Necesitas hacer público este método

            return Ok(new
            {
                token,
                name = user.Name,        // ✅ NUEVO
                bp = user.BP, // ✅ NUEVO
                expiresIn = 3600,
                Result = new
                {
                    Code = "200",
                    Description = "OK"
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error interno del servidor",
                error = ex.Message,
                Result = new
                {
                    Code = "500",
                    Description = "Internal Server Error"
                }
            });
        }
    }

    // POST api/auth/forgot-password
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest("Email requerido");

        var ok = await _auth.SendPasswordResetAsync(request.Email);

        // Por seguridad, no revelar si existe o no
        if (!ok)
            return Ok(new { success = true });

        return Ok(new { success = true });
    }

    // POST api/auth/reset-password
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Token) ||
            string.IsNullOrWhiteSpace(request.NewPassword))
            return BadRequest("Datos inválidos");

        var ok = await _auth.ResetPasswordAsync(request.Token, request.NewPassword);

        if (!ok)
            return BadRequest("Token inválido o expirado");

        return Ok(new { success = true });
    }
}