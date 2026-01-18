using AuthService.API.Data;
using AuthService.API.Dtos;
using AuthService.API.Models;
using AuthService.API.Services;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("admin")]
public class AdminController : ControllerBase
{

    private readonly LatamDbContext _context;
    private readonly AuthServices _authService;

    public AdminController(LatamDbContext context, AuthServices authService)
    {
        _context = context;
        _authService = authService;
    }

    [Authorize(Roles = "admin")]
    [HttpGet("user-by-bp/{bp}")]
    public async Task<IActionResult> GetUserByBp(string bp)
    {
        var user = await _authService.GetUserByBpAsync(bp);
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
        // 1) Traer órdenes + kit desde _context
        var ordersQuery =
            from o in _context.Orders
            join k in _context.KitTypes on o.KitTypeId equals k.KitTypeId
            where o.Status == "Pendiente"
            orderby o.DateOrder descending
            select new
            {
                o.OrderId,
                o.UserId,
                o.TotalPrice,
                KitName = k.Name
            };

        var orders = await ordersQuery.ToListAsync();

        // 2) Traer los usuarios necesarios desde _context
        var userIds = orders.Select(o => o.UserId).Distinct().ToList();

        var users = await _context.Users
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
            from ok in _context.OrderKits
            join g in _context.Garments on ok.GarmentId equals g.GarmentId
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

        var order = await _context.Orders
            .Include(o => o.Tracking) 
            .FirstOrDefaultAsync(o => o.OrderId == request.OrderId);

        if (order == null)
            return NotFound(new { success = false });

        if (order.Status.ToLower() != "pendiente")
            return BadRequest(new { success = false });

        order.Status = accion == "aprobar" ? "Aprobado" : "Rechazado";

        var trackingStatus = _context.TrackingStatus.FirstOrDefault(ts => ts.Name == order.Status);
        if (trackingStatus == null)
            return BadRequest(new { message = "Estado de seguimiento no encontrado" });

        var status = await _context.TrackingStatus
            .FirstAsync(ts => ts.Name == order.Status);

        
        var tracking = new Tracking
        {
            Order = order,  
            TrackingStatusId = status.TrackingStatusId,
            TrackingStatus = trackingStatus,
            TrackingDate = DateTime.UtcNow,
            Observations =  accion == "aprobar" ? "Solicitud Aprobada" : " Solicitud Rechazada"
        };

        _context.Entry(order).State = EntityState.Modified;
        _context.Entry(trackingStatus).State = EntityState.Unchanged;
        _context.Tracking.Add(tracking);
        await _context.SaveChangesAsync();

        return Ok(new { success = true });
    }

}
