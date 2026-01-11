using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AuthService.API.Data;
using AuthService.API.Models;
using AuthService.API.Dtos;
using System.Security.Claims;

namespace AuthService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class KitsController : ControllerBase
{
    private readonly LatamDbContext _context;
    private readonly AuthDbContext _authContext;

    public KitsController(LatamDbContext context, AuthDbContext authContext)
    {
        _context = context;
        _authContext = authContext;
    }

    [HttpPost]
    public async Task<IActionResult> CreateKit([FromBody] CreateKitDto dto)
    {
        try
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new
                {
                    message = "Usuario no autenticado",
                    Result = new
                    {
                        Code = "401",
                        Description = "Unauthorized"
                    }
                });

            var user = await _authContext.Users
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return NotFound(new
                {
                    message = "Usuario no encontrado",
                    Result = new
                    {
                        Code = "404",
                        Description = "Not Found"
                    }
                });

            var kitType = await _context.KitTypes.FindAsync(dto.KitTypeId);

            if (kitType == null)
                return BadRequest(new
                {
                    message = "Tipo de kit no válido",
                    Result = new
                    {
                        Code = "400",
                        Description = "Bad Request"
                    }
                });

            if (dto.Items == null || !dto.Items.Any())
                return BadRequest(new
                {
                    message = "Debe seleccionar al menos una prenda",
                    Result = new
                    {
                        Code = "400",
                        Description = "Bad Request"
                    }
                });

            var kit = new Kit
            {
                KitCode = $"KIT-{DateTime.UtcNow:yyyyMMddHHmmss}",
                KitName = kitType.Name,
                OrderNumber = $"ORD-{DateTime.UtcNow.Ticks}",
                CreatedAt = DateTime.UtcNow,
                Status = "Pending",
                KitTypeId = dto.KitTypeId,
                RequestedByUserId = user.Id
            };

            // Agregar items
            foreach (var itemDto in dto.Items)
            {
                var garment = await _context.Garments.FindAsync(itemDto.GarmentId);
                if (garment == null)
                {
                    Console.WriteLine($"❌ Prenda {itemDto.GarmentId} no encontrada");
                    return BadRequest(new
                    {
                        message = $"Prenda {itemDto.GarmentId} no encontrada",
                        Result = new
                        {
                            Code = "400",
                            Description = "Bad Request"
                        }
                    });
                }

                var kitItem = new KitItem
                {
                    GarmentId = itemDto.GarmentId,
                    SizeId = itemDto.SizeId,
                    Quantity = itemDto.Quantity
                };

                kit.Items.Add(kitItem);
                Console.WriteLine($"   ✅ Item agregado: {garment.Name} x{itemDto.Quantity}");
            }

            // Guardar en BD
            Console.WriteLine($"💾 Guardando en BD...");
            _context.Kits.Add(kit);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✅ Kit guardado exitosamente con ID: {kit.KitId}");

            return Ok(new
            {
                message = "Kit creado exitosamente",
                kitId = kit.KitId,
                orderNumber = kit.OrderNumber,
                createdAt = kit.CreatedAt,
                status = kit.Status,
                Result = new
                {
                    Code = "200",
                    Description = "OK"
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERROR al crear kit: {ex.Message}");
            Console.WriteLine($"❌ Inner Exception: {ex.InnerException?.Message}");
            Console.WriteLine($"❌ Stack Trace: {ex.StackTrace}");

            return StatusCode(500, new
            {
                message = "Error al crear el kit",
                error = ex.Message,
                innerError = ex.InnerException?.Message,
                Result = new
                {
                    Code = "500",
                    Description = "Internal Server Error"
                }
            });
        }
    }

    [HttpGet("my-requests")]
    public async Task<IActionResult> GetMyRequests()
    {
        try
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized(new
                {
                    message = "Usuario no autenticado",
                    Result = new
                    {
                        Code = "401",
                        Description = "Unauthorized"
                    }
                });

            var user = await _authContext.Users
                .FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return NotFound(new
                {
                    message = "Usuario no encontrado",
                    Result = new
                    {
                        Code = "404",
                        Description = "Not Found"
                    }
                });

            var kits = await _context.Kits
                .Include(k => k.KitType)
                .Include(k => k.Items)
                    .ThenInclude(ki => ki.Garment)
                .Where(k => k.RequestedByUserId == user.Id)
                .OrderByDescending(k => k.CreatedAt)
                .ToListAsync();

            if (!kits.Any())
                return NotFound(new
                {
                    message = "No se encontraron solicitudes de kits",
                    Result = new
                    {
                        Code = "404",
                        Description = "Not Found"
                    }
                });

            var response = kits.Select(k => new KitResponseDto
            {
                KitId = k.KitId,
                OrderNumber = k.OrderNumber,
                CreatedAt = k.CreatedAt,
                Status = k.Status,
                KitTypeName = k.KitType?.Name ?? "N/A",
                Items = k.Items.Select(ki => new KitItemResponseDto
                {
                    GarmentName = ki.Garment?.Name ?? "N/A",
                    Quantity = ki.Quantity
                }).ToList()
            }).ToList();

            return Ok(new
            {
                kits = response,
                count = response.Count,
                Result = new
                {
                    Code = "200",
                    Description = "OK"
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error al obtener solicitudes",
                error = ex.Message,
                Result = new
                {
                    Code = "500",
                    Description = "Internal Server Error"
                }
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var kit = await _context.Kits
                .Include(k => k.KitType)
                .Include(k => k.Items)
                    .ThenInclude(ki => ki.Garment)
                .FirstOrDefaultAsync(k => k.KitId == id);

            if (kit == null)
                return NotFound(new
                {
                    message = "Kit no encontrado",
                    Result = new
                    {
                        Code = "404",
                        Description = "Not Found"
                    }
                });

            var response = new KitResponseDto
            {
                KitId = kit.KitId,
                OrderNumber = kit.OrderNumber,
                CreatedAt = kit.CreatedAt,
                Status = kit.Status,
                KitTypeName = kit.KitType?.Name ?? "N/A",
                Items = kit.Items.Select(ki => new KitItemResponseDto
                {
                    GarmentName = ki.Garment?.Name ?? "N/A",
                    Quantity = ki.Quantity
                }).ToList()
            };

            return Ok(new
            {
                kit = response,
                Result = new
                {
                    Code = "200",
                    Description = "OK"
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error al obtener kit",
                error = ex.Message,
                Result = new
                {
                    Code = "500",
                    Description = "Internal Server Error"
                }
            });
        }
    }
}