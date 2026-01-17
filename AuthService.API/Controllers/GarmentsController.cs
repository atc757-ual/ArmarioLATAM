using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AuthService.API.Data;

namespace AuthService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GarmentsController : ControllerBase
{
    private readonly LatamDbContext _context;

    public GarmentsController(LatamDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var garments = await _context.Garments
                .Where(g => g.IsActive)
                .ToListAsync();

            if (!garments.Any())
                return NotFound(new
                {
                    message = "No se encontraron prendas activas",
                    Result = new
                    {
                        Code = "404",
                        Description = "Not Found"
                    }
                });

            return Ok(new
            {
                garments,
                count = garments.Count,
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
                message = "Error al obtener las prendas",
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