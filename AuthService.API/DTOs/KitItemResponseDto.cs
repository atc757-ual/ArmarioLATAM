namespace AuthService.API.Dtos;

public class KitItemResponseDto
{
    public string GarmentName { get; set; } = default!;
    public string SizeCode { get; set; } = default!;
    public int Quantity { get; set; }
}