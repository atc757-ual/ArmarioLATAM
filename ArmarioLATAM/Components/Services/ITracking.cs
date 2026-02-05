using ArmarioLATAM.Components.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace ArmarioLATAM.Services
{
    // ✅ Interface correcto
    public interface ITrackingService
    {
        Task<OrderTrackingResponseDto?> GetOrderTrackingAsync(int orderId);  
    }

    // ✅ Class name = interface name
    public class TrackingService : ITrackingService  // ← NO IOrderService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        private readonly ILogger<TrackingService> _logger;  // ← TrackingService

        public TrackingService(HttpClient httpClient,
                              IAuthService authService,
                              ILogger<TrackingService> logger)  // ← TrackingService
        {
            _httpClient = httpClient;
            _authService = authService;
            _logger = logger;
        }

        private async Task<bool> AttachTokenAsync()
        {
            var token = await _authService.GetTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
                _logger.LogWarning("Sin token válido (Tracking).");
                return false;
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
            return true;
        }
        public async Task<OrderTrackingResponseDto?> GetOrderTrackingAsync(int orderId)
        {
            if (!await AttachTokenAsync()) return null;

            try
            {
                var response = await _httpClient.GetAsync($"api/tracking/order/{orderId}");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<OrderTrackingResponseDto>();
                _logger.LogInformation("✅ Order {OrderId} mapeado", orderId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error order {OrderId}", orderId);
                return null;
            }
        }

    }
}
