
using AuthService.API.Models;
using Xunit;


namespace AuthService.API.Models;

public class Kit
{
    public int KitId { get; set; } // Auto-generado por SQL Server (IDENTITY)
    public string KitCode { get; set; } = default!;
    public string KitName { get; set; } = default!;
    public string? OrderNumber { get; set; } // CAMBIADO: de string a int para coincidir con BD
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Pending";
    public int KitTypeId { get; set; }
    public Guid RequestedByUserId { get; set; } // User.Id es Guid
    public int? MotherTongueLanguageId { get; set; }
    public int? SecondLanguageId { get; set; }
    public int? ThirdLanguageId { get; set; }

    public KitType? KitType { get; set; }
    public List<KitItem> Items { get; set; } = new();
}