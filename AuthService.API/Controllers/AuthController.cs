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
        var user = await _auth.Register(request.Email, request.Password);

        if (user == null)
            return BadRequest(new { message = "Usuario ya existe o datos inválidos" });

        return Ok(new
        {
            message = "Usuario registrado exitosamente",
            userId = user.Id,
            email = user.Email
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var token = await _auth.Login(request.Email, request.Password);

        if (token == null)
            return Unauthorized(new { message = "Credenciales inválidas" });

        return Ok(new
        {
            token,
            expiresIn = 300
        });
    }
}