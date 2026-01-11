using ArmarioLATAM.Components.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ArmarioLATAM.Services
{
    public class GarmentSelectionState
    {
        private const string StorageKey = "garmentSelections";

        private readonly LocalStorageService _localStorage;

        public GarmentSelectionState(LocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        // Estado en memoria
        public List<GarmentSelection> SelectedGarments { get; } = new();

        public bool IsEmpty => !SelectedGarments.Any();

        // Set síncrono en memoria (filtra quantity > 0)
        public void SetSelections(IEnumerable<GarmentSelection> garments)
        {
            SelectedGarments.Clear();
            SelectedGarments.AddRange(
                garments.Where(x => x.Quantity > 0)
            );
        }

        // Guarda estado actual en localStorage
        public async Task SaveAsync()
        {
            var dtoList = SelectedGarments.Select(g => new GarmentSelectionDto
            {
                GarmentId = g.GarmentId,
                Name = g.Name,
                Quantity = g.Quantity,
                Size = g.Size,
                ImageURL = g.ImageURL,
                Languages = g.Languages
            }).ToList();

            var json = JsonSerializer.Serialize(dtoList);
            await _localStorage.SetItemAsync(StorageKey, json);
        }

        // Carga desde localStorage al estado en memoria
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
                SelectedGarments.Add(new GarmentSelection
                {
                    GarmentId = dto.GarmentId,
                    Name = dto.Name,
                    Quantity = dto.Quantity,
                    Size = dto.Size,
                    ImageURL = dto.ImageURL,
                    Languages = dto.Languages
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
            public string? Languages { get; set; }
        }
    }
}
