namespace AuthService.API.Models;

public class OrderItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}