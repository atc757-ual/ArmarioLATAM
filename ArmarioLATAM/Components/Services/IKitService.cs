using ArmarioLATAM.Components.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ArmarioLATAM.Services
{
    public interface IKitService
    {
        Task<List<KitType>?> GetKitTypesAsync();
    }

    public class KitService : IKitService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        private readonly ILogger<KitService> _logger;

        public KitService(HttpClient httpClient,
                          IAuthService authService,
                          ILogger<KitService> logger)
        {
            _httpClient = httpClient;
            _authService = authService;
            _logger = logger;

            _logger.LogInformation("KitService creado. Hash AuthService={Hash}", _authService.GetHashCode());
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

        public async Task<List<KitType>?> GetKitTypesAsync()
        {

            _logger.LogInformation("getkit async");
            if (!await AttachTokenAsync())
                return null; // o lanzar una excepción de sesión no válida

            var response = await _httpClient.GetAsync("api/KitTypes");
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var kitType = await response.Content.ReadFromJsonAsync<KitTypesResponse>();
            if (kitType is null)
                return null;

            _logger.LogInformation("Recibidos {Count} kits", kitType.Count);

            return kitType.KitTypes;
        }
    
    }
}