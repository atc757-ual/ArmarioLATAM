namespace AuthService.API.Models;

public class KitType
{
    public int KitTypeId { get; set; } // CAMBIADO: de Guid a int
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string ImageURL { get; set; } = default!;
    public string ImageURLSelect { get; set; } = default!;
    public bool IsActive { get; set; } = true;
}