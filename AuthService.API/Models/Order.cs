namespace AuthService.API.Models;

public class Order
{
    public int OrderId { get; set; }
    public Guid UserId { get; set; }  
    public DateTime DateOrder { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = default!;
    public int KitTypeId { get; set; }
    public KitType KitType { get; set; } =  null!;
    public AddInfoOrder? AddInfoOrder { get; set; }
    public ICollection<OrderKit> OrderKits { get; set; } = new List<OrderKit>();
    // 👇 NUEVA navegación hacia Tracking
    public ICollection<Tracking> Tracking { get; set; } = new List<Tracking>();
}
