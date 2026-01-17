namespace AuthService.API.Dtos;

public class PendingOrderDto
{
    public int OrderId { get; set; }             
    public string Nombre { get; set; } = string.Empty;   
    public string BP { get; set; } = string.Empty;       
    public decimal Precio { get; set; }                  
    public string Kit { get; set; } = string.Empty;    
}

public class OrderDetailItemDto
{
    public string Nombre { get; set; } = string.Empty;   
    public string? Size { get; set; }  
    public string? Languages { get; set; } 
    public int Cantidad { get; set; }                    
    public decimal PrecioUnitario { get; set; }         
    public decimal Total { get; set; }                   
}

public class UpdateOrderStatusRequest
{
    public int OrderId { get; set; }
    public string Accion { get; set; } = string.Empty; 
}