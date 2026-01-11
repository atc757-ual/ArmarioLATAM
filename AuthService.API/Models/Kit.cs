namespace AuthService.API.Models;

public class Kit
{
    public int KitId { get; set; }
    public string KitCode { get; set; } = default!;
    public string KitName { get; set; } = default!;
    public string OrderNumber { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = default!;
    public int KitTypeId { get; set; }
    public int RequestedByUserId { get; set; } // CAMBIADO: de Guid a int

    // Navegación
    public KitType? KitType { get; set; }
    public User? RequestedByUser { get; set; } // Opcional: navegación al usuario
    public List<KitItem> Items { get; set; } = new();
}