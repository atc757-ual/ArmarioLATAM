namespace AuthService.API.Models;

public class Order
{
    public int OrderId { get; set; }

    public Guid UserId { get; set; }   // 🔴 GUID, NO int

    public DateTime DateOrder { get; set; }

    public decimal TotalPrice { get; set; }

    public string Estado { get; set; } = default!;

    public int KitTypeId { get; set; }
    public KitType KitType { get; set; } = default!;

    public ICollection<OrderKit> OrderKits { get; set; } = new List<OrderKit>();
}
