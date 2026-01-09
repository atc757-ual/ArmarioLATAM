using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AuthService.API.Data;

namespace AuthService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SizesController : ControllerBase
{
    private readonly LatamDbContext _context;

    public SizesController(LatamDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var sizes = await _context.Sizes.ToListAsync();
        return Ok(sizes);
    }
}