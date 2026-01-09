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
        var garments = await _context.Garments
            .Where(g => g.IsActive)
            .ToListAsync();

        return Ok(garments);
    }
}