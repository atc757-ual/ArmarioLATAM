namespace AuthService.API.Models;

public class OrderKit
{
    public int OrderKitId { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; } = default!;
    public int GarmentId { get; set; }
    public Garment Garment { get; set; } = default!;
    public string? Size { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string? Languages { get; set; }
}