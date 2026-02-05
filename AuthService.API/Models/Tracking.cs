namespace AuthService.API.Models
{
    public class Tracking
    {
        public int TrackingId { get; set; }          // Columna TrackingId
        public int OrderId { get; set; }             // Columna OrderId (FK)
        public int TrackingStatusId { get; set; }    // Columna TrackingStatusId (FK)
        public string Observations { get; set; } = string.Empty; // Columna Observation
        public Order Order { get; set; } = new();         // Navigation (EF genera)
        public TrackingStatus TrackingStatus { get; set; } = new();
        public DateTime TrackingDate { get; set; }  // Columna TrackingDate
    }

}