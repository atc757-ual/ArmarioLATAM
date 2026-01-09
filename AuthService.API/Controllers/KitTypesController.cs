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
        var kitTypes = await _context.KitTypes.ToListAsync();
        return Ok(kitTypes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) // CAMBIADO: de Guid a int
    {
        var kitType = await _context.KitTypes.FindAsync(id);

        if (kitType == null)
            return NotFound(new { message = "Tipo de kit no encontrado" });

        return Ok(kitType);
    }
}