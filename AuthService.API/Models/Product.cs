namespace AuthService.API.Models;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public int Stock { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
}