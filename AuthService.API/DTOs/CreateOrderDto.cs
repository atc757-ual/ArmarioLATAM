namespace AuthService.API.Dtos;

public class CreateOrderDto
{
    public List<OrderItemDto> Items { get; set; } = new();
}