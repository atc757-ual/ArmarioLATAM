namespace AuthService.API.Dtos;

public class KitResponseDto
{
    public int KitId { get; set; } 
    public string OrderNumber { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = default!;
    public string KitTypeName { get; set; } = default!;
    public List<KitItemResponseDto> Items { get; set; } = new();
}