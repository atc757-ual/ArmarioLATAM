using System.Text.Json;

public class OrderState
{
    private const string StorageKey = "lastOrder";

    private readonly SessionStorageService _localStorage;

    public OrderState(SessionStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public string? OrderNumber { get; private set; }
    public string? OrderDate { get; private set; }

    public bool IsEmpty =>
        string.IsNullOrEmpty(OrderNumber) && string.IsNullOrEmpty(OrderDate);

    public void Set(string orderNumber, string orderDate)
    {
        OrderNumber = orderNumber;
        OrderDate = orderDate;
    }

    public async Task SaveAsync()
    {
        var dto = new LastOrderDto
        {
            OrderNumber = OrderNumber,
            OrderDate = OrderDate
        };

        var json = JsonSerializer.Serialize(dto);
        await _localStorage.SetItemAsync(StorageKey, json);
    }

    public async Task LoadAsync()
    {
        var json = await _localStorage.GetItemAsync(StorageKey);
        if (string.IsNullOrWhiteSpace(json))
            return;

        var dto = JsonSerializer.Deserialize<LastOrderDto>(json);
        if (dto is null)
            return;

        OrderNumber = dto.OrderNumber;
        OrderDate = dto.OrderDate;
    }

    public async Task ClearAsync()
    {
        OrderNumber = null;
        OrderDate = null;
        await _localStorage.RemoveItemAsync(StorageKey);
    }

    private class LastOrderDto
    {
        public string? OrderNumber { get; set; }
        public string? OrderDate { get; set; }
    }
}
