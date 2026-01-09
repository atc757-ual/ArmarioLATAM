namespace AuthService.API.Models;

public class Size
{
    public int SizeId { get; set; } // CAMBIADO: de Guid a int
    public string Code { get; set; } = default!;
    public string? Description { get; set; }
}