namespace AuthService.API.Models;

public class Garment
{
    public int GarmentId { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public int QuantityAuth { get; set; }
    public string? Sizes { get; set; }
    public string? ImageURL { get; set; }
    public string? Languages { get; set; }
    public decimal Price { get; set; }
}