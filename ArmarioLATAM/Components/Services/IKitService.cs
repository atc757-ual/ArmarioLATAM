using ArmarioLATAM.Components.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ArmarioLATAM.Services
{
    public interface IKitService
    {
        Task<List<KitType>?> GetKitTypesAsync();
        Task<KitType?> GetKitTypeByIdAsync(int id);
        void SetSelectedKitTypeId(int kitTypeId);
        int? GetSelectedKitTypeId();
    }

    public class KitService : IKitService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        private readonly ILogger<KitService> _logger;
        private int? _selectedKitTypeId;

        public KitService(HttpClient httpClient,
                          IAuthService authService,
                          ILogger<KitService> logger)
        {
            _httpClient = httpClient;
            _authService = authService;
            _logger = logger;

            _logger.LogInformation("KitService creado. Hash AuthService={Hash}", _authService.GetHashCode());
        }

        private async Task AttachTokenAsync()
        {
            // Si has añadido GetTokenAsync en IAuthService, úsalo aquí:
            var token = (_authService as AuthService) is not null
                ? await ((AuthService)_authService).GetTokenAsync()
                : _authService.GetToken();

            _logger.LogWarning("AttachToken: token = {Token}", token);

            if (!string.IsNullOrWhiteSpace(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
                _logger.LogWarning("NO hay header Authorization en HttpClient.");
            }
        }

        public async Task<List<KitType>?> GetKitTypesAsync()
        {
            _logger.LogInformation("?? Entrando a GetKitTypesAsync");

            await AttachTokenAsync();

            var response = await _httpClient.GetAsync("api/KitTypes");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _logger.LogWarning("Respuesta 401 Unauthorized desde api/KitTypes");
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Llamada a api/KitTypes falló con StatusCode {StatusCode}", response.StatusCode);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<List<KitType>>();
        }
    

        public async Task<KitType?> GetKitTypeByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/KitTypes/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<KitType>();
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error obteniendo kit {id}");
                return null;
            }
        }

        public void SetSelectedKitTypeId(int kitTypeId)
        {
            _selectedKitTypeId = kitTypeId;
            _logger.LogInformation($"✅ KitTypeId guardado: {kitTypeId}");
        }

        public int? GetSelectedKitTypeId()
        {
            return _selectedKitTypeId;
        }
    }
}