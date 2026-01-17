using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AuthService.API.Data;

namespace AuthService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class KitTypesController : ControllerBase
{
    private readonly LatamDbContext _context;

    public KitTypesController(LatamDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var kitTypes = await _context.KitTypes.
                Where(k => k.IsActive).
                ToListAsync();

            if (!kitTypes.Any())
                return NotFound(new
                {
                    message = "No se encontraron tipos de kit",
                    Result = new
                    {
                        Code = "404",
                        Description = "Not Found"
                    }
                });

            return Ok(new
            {
                kitTypes,
                count = kitTypes.Count,
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
                message = "Error al obtener los tipos de kit",
                error = ex.Message,
                Result = new
                {
                    Code = "500",
                    Description = "Internal Server Error"
                }
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var kitType = await _context.KitTypes.FindAsync(id);

            if (kitType == null)
                return NotFound(new
                {
                    message = "Tipo de kit no encontrado",
                    Result = new
                    {
                        Code = "404",
                        Description = "Not Found"
                    }
                });

            return Ok(new
            {
                kitType,
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
                message = "Error al obtener el tipo de kit",
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