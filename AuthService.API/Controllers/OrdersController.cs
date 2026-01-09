using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AuthService.API.Data;
using AuthService.API.Models;
using System.Security.Claims;

namespace AuthService.API.Controllers;

[ApiController]
[Route("orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly OrderDbContext _context;

    public OrdersController(OrderDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Order order)
    {
        order.OrderNumber = $"ORD-{DateTime.UtcNow.Ticks}";
        order.UserEmail = User.FindFirstValue(ClaimTypes.Email)!;
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return Ok(order);
    }
}