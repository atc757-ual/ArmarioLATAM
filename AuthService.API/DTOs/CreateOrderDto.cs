namespace AuthService.API.Dtos;

public class CreateOrderDto
{
    public int KitTypeId { get; set; }
    public List<CreateOrderItemDto> Items { get; set; } = new();
    public string? Motive { get; set; }
    public string? DetailMotive { get; set; }
    public string? Province { get; set; }
    public string? District { get; set; }
    public string? Address { get; set; }
    public string? AddressReference { get; set; }
}
