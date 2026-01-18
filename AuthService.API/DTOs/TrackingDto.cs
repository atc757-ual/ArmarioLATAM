using AuthService.API.Models;

namespace AuthService.API.Dtos;
public class OrderTrackingResponseDto
{
    public int OrderId { get; set; }
    public string KitTypeName { get; set; } = "";
    public List<TrackingDto_> Tracking { get; set; } = new();
    public List<OrderKitDto_> OrderKits { get; set; } = new();
}

public class TrackingDto_
{

    public int TrackingStatusId { get; set; }
    public string? TrackingStatusDescription { get; set; } = string.Empty;
    public string TrackingStatusName { get; set; } = string.Empty;
    public DateTime? TrackingDate { get; set; }
}

public class OrderKitDto_
{
    public string ImageUrl { get; set; } = string.Empty;
    public string GarmentName { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string Languages { get; set; } = string.Empty;
    public int Quantity { get; set; }
}