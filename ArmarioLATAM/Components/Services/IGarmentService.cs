using ArmarioLATAM.Components.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ArmarioLATAM.Services
{
    public interface IGarmentService
    {
        Task<List<Garment>?> GetGarmentsAsync();
        Task<Garment?> GetGarmentByIdAsync(int id);
        void SetSelectedGarmentId(int garmentId);
        int? GetSelectedGarmentId();
    }

    public class GarmentService : IGarmentService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        private readonly ILogger<GarmentService> _logger;
        private int? _selectedGarmentId;

        public GarmentService(HttpClient httpClient,
                              IAuthService authService,
                              ILogger<GarmentService> logger)
        {
            _httpClient = httpClient;
            _authService = authService;
            _logger = logger;

            _logger.LogInformation("GarmentService creado. Hash AuthService={Hash}", _authService.GetHashCode());
        }


        private async Task<bool> AttachTokenAsync()
        {
            var token = await _authService.GetTokenAsync(); // ya filtra caducados

            if (string.IsNullOrWhiteSpace(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
                _logger.LogWarning("Sin token válido, NO se llama al API.");
                return false;
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            return true;
        }

        public async Task<List<Garment>?> GetGarmentsAsync()
        {
            _logger.LogInformation("➡️ Entrando a GetGarmentsAsync");

            await AttachTokenAsync();

            var response = await _httpClient.GetAsync("api/garments");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _logger.LogWarning("GetGarmentsAsync: 401 Unauthorized desde api/garments");
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("GetGarmentsAsync falló con StatusCode {StatusCode}", response.StatusCode);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<List<Garment>>();
        }

        public async Task<Garment?> GetGarmentByIdAsync(int id)
        {
            try
            {
                await AttachTokenAsync();

                var response = await _httpClient.GetAsync($"api/garments/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Garment>();
                }

                _logger.LogWarning("GetGarmentByIdAsync({Id}) devolvió StatusCode {StatusCode}", id, response.StatusCode);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo garment {Id}", id);
                return null;
            }
        }

        public void SetSelectedGarmentId(int garmentId)
        {
            _selectedGarmentId = garmentId;
            _logger.LogInformation("✅ GarmentId guardado: {GarmentId}", garmentId);
        }

        public int? GetSelectedGarmentId()
        {
            return _selectedGarmentId;
        }
    }
}
