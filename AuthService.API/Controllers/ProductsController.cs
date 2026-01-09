using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AuthService.API.Data;

namespace AuthService.API.Controllers;

[ApiController]
[Route("products")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly ProductDbContext _context;

    public ProductsController(ProductDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _context.Products.Where(p => p.IsActive).ToListAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        return product == null ? NotFound() : Ok(product);
    }
}