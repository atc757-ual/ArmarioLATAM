using AuthService.API.Data;
using AuthService.API.Dtos;
using AuthService.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TrackingController : ControllerBase
{
    private readonly LatamDbContext _context;

    public TrackingController(LatamDbContext context) => _context = context;


    /// API 2: Tracking ESPECÍFICO de UNA orden (valida ownership)
    [HttpGet("order/{orderId}")]
    public async Task<ActionResult<OrderTrackingResponseDto>> GetCompleteOrderTracking(int orderId)
    {
        // 1. Valida UserId del token
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized();

        // 2. Verifica orden pertenece al usuario
        var ownsOrder = await _context.Orders
            .AnyAsync(o => o.OrderId == orderId && o.UserId.ToString() == userIdClaim);

        if (!ownsOrder)
            return BadRequest(new { message = "Orden no encontrada" });

        var order = await _context.Orders
               .Include(o => o.KitType)
               .Include(o => o.OrderKits)
                   .ThenInclude(ok => ok.Garment)  // ✅ Imagen aquí
               .Include(o => o.Tracking).ThenInclude(t => t.TrackingStatus)
               .FirstOrDefaultAsync(o => o.OrderId == orderId);

        var result = new OrderTrackingResponseDto();


        if (order is not null)
        {
            result = new OrderTrackingResponseDto
            {
                OrderId = order.OrderId,
                KitTypeName = order.KitType?.Description ?? "Sin Tipo",
                Tracking = order.Tracking?.Select(t => new TrackingDto_
                {
                    TrackingStatusId = t.TrackingStatusId,
                    TrackingStatusName = t.TrackingStatus?.Name ?? "Sin Status",
                    TrackingStatusDescription = t.TrackingStatus?.Description ?? "Sin Status",
                    TrackingDate = t.TrackingDate
                }).OrderBy(t => t.TrackingDate).ToList() ?? new List<TrackingDto_>
        {
            new() { TrackingStatusDescription = "Pendiente", TrackingDate = DateTime.UtcNow }
        },
                OrderKits = order.OrderKits.Select(ok => new OrderKitDto_
                {
                    Size = ok.Size ?? "",
                    Languages = ok.Languages ?? "",
                    Quantity = ok.Quantity,
                    ImageUrl = ok.Garment.ImageUrl ?? "",
                    GarmentName = ok.Garment?.Name ?? ""
                }).ToList()
            };
            return Ok(result);
        }
        else
        {
            return result;
        }

      
    }
}
