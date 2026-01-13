using ArmarioLATAM.Components.Models;
using System.Text.Json;

public class DetailDeliveredState
{
    private const string StorageKey = "detailDelivered";
    private readonly SessionStorageService _sessionStorage;

    public DetailDeliveredState(SessionStorageService sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    // Estado en memoria
    public DetailDelivered Detail { get; private set; } = new();

    public bool IsEmpty =>
        string.IsNullOrWhiteSpace(Detail.Motive) &&
        string.IsNullOrWhiteSpace(Detail.DetailMotive) &&
        string.IsNullOrWhiteSpace(Detail.Province) &&
        string.IsNullOrWhiteSpace(Detail.District) &&
        string.IsNullOrWhiteSpace(Detail.Address);

    // Actualiza el estado en memoria
    public void SetDetail(DetailDelivered detail)
    {
        Detail = detail ?? new DetailDelivered();
    }

    // Guarda estado actual en SessionStorage (JSON)
    public async Task SaveAsync()
    {
        var json = JsonSerializer.Serialize(Detail);
        await _sessionStorage.SetItemAsync(StorageKey, json);
    }

    // Carga desde SessionStorage al estado en memoria
    public async Task LoadAsync()
    {
        var json = await _sessionStorage.GetItemAsync(StorageKey);
        if (string.IsNullOrWhiteSpace(json))
            return;

        var loaded = JsonSerializer.Deserialize<DetailDelivered>(json);
        if (loaded is null)
            return;

        Detail = loaded;
    }

    public async Task ClearAsync()
    {
        Detail = new DetailDelivered();
        await _sessionStorage.RemoveItemAsync(StorageKey);
    }
}
