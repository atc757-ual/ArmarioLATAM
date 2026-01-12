namespace AuthService.API.Dtos;

public class CreateOrderItemDto
{
    public int GarmentId { get; set; }
    public string Size { get; set; } = default!;
    public int Quantity { get; set; }
}
