using ArmarioLATAM.Components.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
public class KitSelectionState
{
    private const string StorageKey = "kitSelection";

    private readonly LocalStorageService _localStorage;

    public KitSelectionState(LocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    // Estado en memoria
    public int? KitTypeId { get; private set; }
    public string? Name { get; private set; }
    public string? Description { get; private set; }

    public bool IsEmpty =>
        KitTypeId is null && string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(Description);

    // Set síncrono en memoria
    public void Set(KitType kit)
    {
        KitTypeId = kit.KitTypeId;
        Name = kit.Name;
        Description = kit.Description;
    }

    // Guarda estado actual en localStorage
    public async Task SaveAsync()
    {
        var dto = new
        {
            KitTypeId,
            Name,
            Description
        };

        var json = JsonSerializer.Serialize(dto);
        await _localStorage.SetItemAsync(StorageKey, json);
    }

    // Carga desde localStorage al estado en memoria
    public async Task LoadAsync()
    {
        var json = await _localStorage.GetItemAsync(StorageKey);
        if (string.IsNullOrWhiteSpace(json))
            return;

        var dto = JsonSerializer.Deserialize<KitSelectionDto>(json);
        if (dto is null)
            return;

        KitTypeId = dto.KitTypeId;
        Name = dto.Name;
        Description = dto.Description;
    }

    public async Task ClearAsync()
    {
        KitTypeId = null;
        Name = null;
        Description = null;
        await _localStorage.RemoveItemAsync(StorageKey);
    }

    private class KitSelectionDto
    {
        public int? KitTypeId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
