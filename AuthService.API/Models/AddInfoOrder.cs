namespace AuthService.API.Models;

public class AddInfoOrder
{
    public int AddInfoOrderId { get; set; }
    public string? Motive { get; set; }
    public string? DetailMotive { get; set; }
    public string? Province { get; set; }
    public string? District { get; set; }
    public string? Address { get; set; }
    public string? AddressReference { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
}
