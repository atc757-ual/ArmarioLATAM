namespace AuthService.API.Models;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserEmail { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string OrderNumber { get; set; } = default!;
    public List<OrderItem> Items { get; set; } = new();
}