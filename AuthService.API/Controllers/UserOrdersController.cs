using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AuthService.API.Data;
using System.Security.Claims;

namespace AuthService.API.Controllers;

[ApiController]
[Route("user/orders")]
[Authorize]
public class UserOrdersController : ControllerBase
{
    private readonly OrderDbContext _context;

    public UserOrdersController(OrderDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyOrders()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        var orders = await _context.Orders
            .Where(o => o.UserEmail == email)
            .ToListAsync();
        return Ok(orders);
    }
}