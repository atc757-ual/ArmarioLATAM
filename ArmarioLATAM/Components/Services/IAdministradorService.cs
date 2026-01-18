// Services/AdminService.cs
using ArmarioLATAM.Components.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Net.Http.Json;
namespace ArmarioLATAM.Services
{
    public interface IAdminService
    {
        Task<AdminUser?> GetUserByBpAsync(string bp);
        Task<List<PendingOrderDto>> GetPendingOrdersAsync();
        Task<List<OrderDetailItemDto>?> GetOrderDetailAsync(int orderId);
        Task<bool> UpdateOrderStatusAsync(int orderId, string accion); // "aprobar"/"rechazar"
    }

    public class AdminService : IAdminService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        private readonly ILogger<GarmentService> _logger;


        public AdminService(HttpClient httpClient,
                              IAuthService authService,
                              ILogger<GarmentService> logger)
        {
            _httpClient = httpClient;
            _authService = authService;
            _logger = logger;


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
        public async Task<AdminUser?> GetUserByBpAsync(string bp)
        {
            if (!await AttachTokenAsync())
                return null;
            var response = await _httpClient.GetAsync($"admin/user-by-bp/{bp}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation("success");
                return null;
            }
            else
            {
                _logger.LogInformation("error");
                return await response.Content.ReadFromJsonAsync<AdminUser>();
            }
        }
        public async Task<List<PendingOrderDto>> GetPendingOrdersAsync()
        {
            if (!await AttachTokenAsync())
                return new();

            var result = await _httpClient.GetFromJsonAsync<List<PendingOrderDto>>(
                "admin/pending-orders");

            return result ?? new List<PendingOrderDto>();
        }

        public async Task<List<OrderDetailItemDto>?> GetOrderDetailAsync(int orderId)
        {
            if (!await AttachTokenAsync())
                return new();

           var response = await _httpClient.GetFromJsonAsync<List<OrderDetailItemDto>>(
               $"admin/order-detail/{orderId}");
           return response;
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string accion)
        {
            if (!await AttachTokenAsync())
                return false; 
            
            var request = new { OrderId = orderId, Accion = accion };

            var response = await _httpClient.PostAsJsonAsync(
                "admin/update-order-status", request);

            if (!response.IsSuccessStatusCode)
                return false;

            var body = await response.Content.ReadFromJsonAsync<UpdateStatusResponse>();
            return body?.Success == true;
        }

        private sealed class UpdateStatusResponse
        {
            public bool Success { get; set; }
        }
    }
}