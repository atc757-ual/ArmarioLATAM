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
            var user = await _auth.Register(request.Email, request.Password);

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
            var token = await _auth.Login(request.Email, request.Password);

            if (token == null)
                return Unauthorized(new
                {
                    message = "Credenciales inválidas",
                    Result = new
                    {
                        Code = "401",
                        Description = "Unauthorized"
                    }
                });

            return Ok(new
            {
                token,
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
}