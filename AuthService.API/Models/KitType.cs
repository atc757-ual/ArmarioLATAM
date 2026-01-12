namespace AuthService.API.Models;

public class KitType
{
    public int KitTypeId { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public string? ImageURL { get; set; }
    public string? ImageURLSelect { get; set; }
    public string? KitCode { get; set; }
}
