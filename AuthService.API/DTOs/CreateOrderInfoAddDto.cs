namespace AuthService.API.Dtos;

public class CreateOrderInfoAddDto
{
    public int AddInfoOrderId { get; set; }
    public string? Motive { get; set; }
    public string? DetailMotive { get; set; }
    public string? Province { get; set; }
    public string? District { get; set; }
    public string? Address { get; set; }
    public string? AddressReference { get; set; }
    public int OrderId { get; set; }
}
