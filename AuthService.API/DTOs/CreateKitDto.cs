namespace AuthService.API.Dtos;

public class CreateKitDto
{
    public int KitTypeId { get; set; }
    public List<KitItemDto> Items { get; set; } = new();
}