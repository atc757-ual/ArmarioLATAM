namespace AuthService.API.Models;

public class Garment
{
    public int GarmentId { get; set; } // CAMBIADO: de Guid a int
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}