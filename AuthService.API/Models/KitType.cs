namespace AuthService.API.Models;

public class KitType
{
    public int KitTypeId { get; set; } // CAMBIADO: de Guid a int
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}