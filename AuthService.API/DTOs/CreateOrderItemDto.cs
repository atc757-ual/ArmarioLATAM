namespace AuthService.API.Dtos;

public class CreateOrderItemDto
{
    public int GarmentId { get; set; }
    public string? Size { get; set; } 
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public string? Languages { get; set; } 
}
