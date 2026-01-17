using AuthService.API.Data;
using AuthService.API.Dtos;
using AuthService.API.Services;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

[ApiController]
[Route("admin")]
public class AdminController : ControllerBase
{
    private readonly AuthService.API.Services.AuthService _auth;
    private readonly AuthDbContext _authDb;
    private readonly LatamDbContext _latamDb;

    // 👇 Inyecta los 3 servicios/contexts
    public AdminController(
        AuthService.API.Services.AuthService auth,
        AuthDbContext authDb,
        LatamDbContext latamDb)
    {
        _auth = auth;
        _authDb = authDb;
        _latamDb = latamDb;
    }

    [Authorize(Roles = "admin")]
    [HttpGet("user-by-bp/{bp}")]
    public async Task<IActionResult> GetUserByBp(string bp)
    {
        var user = await _auth.GetUserByBpAsync(bp);
        if (user == null) return NotFound();

        return Ok(new UserByBpResponse
        {
            Name = user.Name ?? string.Empty,
            Genero = user.Gender ?? string.Empty,
            Correo = user.Email ?? string.Empty,
            Documento = user.DNI ?? string.Empty
        });
    }

    [Authorize(Roles = "admin")]
    [HttpGet("pending-orders")]
    public async Task<IActionResult> GetPendingOrders()
    {
        // 1) Traer órdenes + kit desde _latamDb
        var ordersQuery =
            from o in _latamDb.Orders
            join k in _latamDb.KitTypes on o.KitTypeId equals k.KitTypeId
            where o.Status == "Pendiente"
            select new
            {
                o.OrderId,
                o.UserId,
                o.TotalPrice,
                KitName = k.Name
            };

        var orders = await ordersQuery.ToListAsync();

        // 2) Traer los usuarios necesarios desde _authDb
        var userIds = orders.Select(o => o.UserId).Distinct().ToList();

        var users = await _authDb.Users
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new
            {
                u.Id,
                u.Name,
                u.BP
            })
            .ToListAsync();

        // 3) Hacer join en memoria
        var result =
            (from o in orders
             join u in users on o.UserId equals u.Id
             select new PendingOrderDto
             {
                 OrderId = o.OrderId,
                 Nombre = u.Name ?? string.Empty,
                 BP = u.BP ?? string.Empty,
                 Precio = o.TotalPrice,
                 Kit = o.KitName
             }).ToList();

        return Ok(result);
    }


    [Authorize(Roles = "admin")]
    [HttpGet("order-detail/{orderId:int}")]
    public async Task<IActionResult> GetOrderDetail(int orderId)
    {
        var query =
            from ok in _latamDb.OrderKits
            join g in _latamDb.Garments on ok.GarmentId equals g.GarmentId
            where ok.OrderId == orderId
            select new OrderDetailItemDto
            {
                Nombre = g.Name,
                Size = ok.Size,
                Languages = ok.Languages,
                Cantidad = ok.Quantity,
                PrecioUnitario = ok.Price,
                Total = ok.Price * ok.Quantity
            };

        var items = await query.ToListAsync();

        if (!items.Any())
            return NotFound();

        return Ok(items);
    }


    [Authorize(Roles = "admin")]
    [HttpPost("update-order-status")]
    public async Task<IActionResult> UpdateOrderStatus([FromBody] UpdateOrderStatusRequest request)
    {
        var accion = request.Accion?.Trim().ToLowerInvariant();
        if (accion != "aprobar" && accion != "rechazar")
            return BadRequest(new { success = false });

        var order = await _latamDb.Orders
            .FirstOrDefaultAsync(o => o.OrderId == request.OrderId);

        if (order == null)
            return NotFound(new { success = false });

        if (order.Status.ToLower() != "pendiente")
            return BadRequest(new { success = false });

        order.Status = accion == "aprobar" ? "Aprobado" : "Rechazado";
        await _latamDb.SaveChangesAsync();

        return Ok(new { success = true });
    }


[Authorize(Roles = "admin")]
[HttpPost("login-as-user/{bp}")]
public async Task<IActionResult> LoginAsUser(string bp)
{
    var user = await _auth.GetUserByBpAsync(bp);
    if (user == null) 
        return NotFound(new { message = "Usuario no encontrado" });

    var token = _auth.GenerateJwt(user);

    return Ok(new
    {
        token,
        name = user.Name,
        bp = user.BP,
        expiresIn = 3600,
        role = user.Role,
        gender = user.Gender,
        birthDate = user.BirthDate,
        activationDate = user.ActivationDate,
        Result = new
        {
            Code = "200",
            Description = "OK"
        }
    });
}
}
