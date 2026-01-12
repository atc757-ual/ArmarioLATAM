namespace AuthService.API.Dtos;

public class CreateOrderDto
{
    public int KitTypeId { get; set; }
    public List<CreateOrderItemDto> Items { get; set; } = new();
}
