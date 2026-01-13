using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AuthService.API.Data;
using AuthService.API.Dtos;
using System.Security.Claims;
using AuthService.API.Models;

namespace AuthService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly LatamDbContext _context;
    private readonly AuthDbContext _authContext;

    public OrdersController(LatamDbContext context, AuthDbContext authContext)
    {
        _context = context;
        _authContext = authContext;
    }

    // ============================
    // CREAR ORDEN
    // ============================
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
    {
        try
        {
            // 🔐 Obtener email desde JWT
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new { message = "Usuario no autenticado" });

            // 👤 Buscar usuario
            var user = await _authContext.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            // 📦 Validar KitType
            var kitType = await _context.KitTypes.FindAsync(dto.KitTypeId);
            if (kitType == null)
                return BadRequest(new { message = "Tipo de kit no válido" });

            if (dto.Items == null || !dto.Items.Any())
                return BadRequest(new { message = "Debe seleccionar al menos una prenda" });

            decimal totalPrice = 0;

            // 🧾 Crear Orden
            var order = new Models.Order
            {
                UserId = user.Id,               // GUID del usuario
                DateOrder = DateTime.UtcNow,
                Status = "Pendiente",
                KitTypeId = dto.KitTypeId,
                TotalPrice = 0
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(); // genera OrderId

            // 📌 Crear items
            foreach (var item in dto.Items)
            {
                var garment = await _context.Garments.FindAsync(item.GarmentId);
                if (garment == null)
                    return BadRequest(new { message = $"Prenda {item.GarmentId} no encontrada" });

                var price = garment.Price * item.Quantity;
                totalPrice += price;

                var orderItem = new OrderKit
                {
                    OrderId = order.OrderId,
                    GarmentId = item.GarmentId,
                    Size = item.Size,
                    Quantity = item.Quantity,
                    Price = garment.Price,
                    Languages = garment.Languages
                };

                _context.OrderKits.Add(orderItem);
            }

            // 💰 Actualizar total
            order.TotalPrice = totalPrice;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Orden creada exitosamente",
                orderId = order.OrderId,
                total = order.TotalPrice,
                status = order.Status,
                fecha = order.DateOrder.ToString("yyyy-MM-dd") // solo fecha
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error al crear la orden",
                error = ex.Message,
                innerError = ex.InnerException?.Message
            });
        }
    }

    // ============================
    // MIS ORDENES
    // ============================
    [HttpGet("my-orders")]
    public async Task<IActionResult> GetMyOrders()
    {
        try
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized();

            var user = await _authContext.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
                return NotFound();

            var orders = await _context.Orders
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.DateOrder)
                .Select(o => new
                {
                    o.OrderId,
                    fecha = o.DateOrder.ToString("yyyy-MM-dd"), // formateamos
                    o.Status,
                    o.TotalPrice,
                    KitType = o.KitType.Name,
                    o.KitType.KitCode

                })
                .ToListAsync();

            return Ok(new
            {
                count = orders.Count,
                orders
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error al obtener órdenes",
                error = ex.Message
            });
        }
    }


    [HttpGet("my-orders-history")]
    public async Task<IActionResult> GetMyOrdersNoPending()
    {
        try
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized();

            var user = await _authContext.Users
                .FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
                return NotFound();

            var orders = await _context.Orders
                .Where(o => o.UserId == user.Id)
                .Where(o => o.Status != "Pendiente")   // <-- solo estados distintos de Pending
                .OrderByDescending(o => o.DateOrder)
                .Select(o => new
                {
                    o.OrderId,
                    fecha = o.DateOrder.ToString("dd/MM/yyyy HH:mm"),
                    o.Status,
                    o.TotalPrice,
                    KitType = o.KitType.Name,
                    o.KitType.KitCode
                })
                .ToListAsync();

            return Ok(new
            {
                count = orders.Count,
                orders
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error al obtener órdenes",
                error = ex.Message
            });
        }
    }

    [HttpGet("my-order-pending")]
    public async Task<IActionResult> GetMyOrders([FromQuery] int? kitTypeId)
    {
        try
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized();

            var user = await _authContext.Users
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return NotFound();

            var query = _context.Orders
                .Where(o => o.UserId == user.Id)
                .Where(o => o.Status == "Pendiente");

            if (kitTypeId.HasValue)
            {
                query = query.Where(o => o.KitTypeId == kitTypeId.Value);
            }

            var order = await query
                .OrderByDescending(o => o.DateOrder)
                .Select(o => new
                {
                    o.OrderId,
                    fecha = o.DateOrder.ToString("dd/MM/yyyy HH:mm"),
                    o.Status,
                    o.TotalPrice,
                    KitType = o.KitType.Name,
                    o.KitType.KitCode
                })
                .FirstOrDefaultAsync(); // <-- solo uno

            if (order is null)
            {
                // No hay orden pendiente para ese usuario/kit
                return Ok(new { count = 0, order = (object?)null });
            }

            return Ok(new
            {
                count = 1,
                order
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error al obtener órdenes",
                error = ex.Message
            });
        }
    }


    // ============================
    // DETALLE DE ORDEN
    // ============================
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        try
        {
            var order = await _context.Orders
                .Include(o => o.OrderKits)
                    .ThenInclude(ok => ok.Garment)
                .Include(o => o.KitType)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return NotFound(new { message = "Orden no encontrada" });

            var response = new
            {
                order.OrderId,
                fecha = order.DateOrder.ToString("dd/MM/yyyy HH:mm"), // solo fecha
                order.Status,
                order.TotalPrice,
                KitType = order.KitType.Name,
                Items = order.OrderKits.Select(i => new
                {
                    Garment = i.Garment.Name,
                    i.Size,
                    i.Quantity,
                    i.Price
                })
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error al obtener orden",
                error = ex.Message
            });
        }
    }
}
