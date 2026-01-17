using ArmarioLATAM.Components.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

public class GarmentSelectionState
{
    private const string StorageKey = "garmentSelections";
    public bool IsEmpty => !SelectedGarments.Any();
    private readonly SessionStorageService _localStorage;

    public GarmentSelectionState(SessionStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    // Estado en memoria
    public List<GarmentSelection> SelectedGarments { get; } = new();

    // Set síncrono en memoria (filtra quantity > 0)
    public void SetSelections(IEnumerable<GarmentSelection> garments)
    {
        SelectedGarments.Clear();
        SelectedGarments.AddRange(
            garments.Where(x => x.Quantity > 0)
        );
    }

    // Guarda estado actual en SessionStorage
    public async Task SaveAsync()
    {
        var dtoList = SelectedGarments.Select(g => new GarmentSelectionDto
        {
            GarmentId = g.GarmentId,
            Name = g.Name,
            Quantity = g.Quantity,
            Size = g.Size,
            ImageURL = g.ImageURL,
            // concatenamos idiomas: "EN,ES,PT"
            Languages = string.Join(",",
                new[] { g.NativeLanguage, g.SecondLanguage, g.ThirdLanguage }
                    .Where(x => !string.IsNullOrWhiteSpace(x)))
        }).ToList();

        var json = JsonSerializer.Serialize(dtoList);
        await _localStorage.SetItemAsync(StorageKey, json);
    }

    // Carga desde SessionStorage al estado en memoria
    public async Task LoadAsync()
    {
        var json = await _localStorage.GetItemAsync(StorageKey);
        if (string.IsNullOrWhiteSpace(json))
            return;

        var dtoList = JsonSerializer.Deserialize<List<GarmentSelectionDto>>(json);
        if (dtoList is null)
            return;

        SelectedGarments.Clear();

        foreach (var dto in dtoList)
        {
            string? native = null, second = null, third = null;

            if (!string.IsNullOrWhiteSpace(dto.Languages))
            {
                var parts = dto.Languages
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .ToArray();

                native = parts.Length > 0 ? parts[0] : null;
                second = parts.Length > 1 ? parts[1] : null;
                third = parts.Length > 2 ? parts[2] : null;
            }

            SelectedGarments.Add(new GarmentSelection
            {
                GarmentId = dto.GarmentId,
                Name = dto.Name,
                Quantity = dto.Quantity,
                Size = dto.Size,
                ImageURL = dto.ImageURL,
                NativeLanguage = native,
                SecondLanguage = second,
                ThirdLanguage = third
            });
        }
    }

    public async Task ClearAsync()
    {
        SelectedGarments.Clear();
        await _localStorage.RemoveItemAsync(StorageKey);
    }

    private class GarmentSelectionDto
    {
        public int GarmentId { get; set; }
        public string? Name { get; set; }
        public int Quantity { get; set; }
        public string? Size { get; set; }
        public string? ImageURL { get; set; }

        // string concatenado "EN,ES,PT"
        public string? Languages { get; set; }
    }
}
