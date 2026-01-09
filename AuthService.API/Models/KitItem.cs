namespace AuthService.API.Models;

public class KitItem
{
    public int KitItemId { get; set; } // Auto-generado por SQL Server (IDENTITY)
    public int KitId { get; set; }
    public int GarmentId { get; set; }
    public int SizeId { get; set; }
    public int Quantity { get; set; }

    public Kit? Kit { get; set; }
    public Garment? Garment { get; set; }
    public Size? Size { get; set; }
}