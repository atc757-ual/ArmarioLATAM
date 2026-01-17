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
        Task<List<OrderDetailItemDto>> GetOrderDetailAsync(int orderId);
        Task<bool> UpdateOrderStatusAsync(int orderId, string accion);
        Task<LoginResponse?> LoginAsUserAsync(string bp);
    }

    public class AdminService : IAdminService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        private readonly ILogger<AdminService> _logger;

        public AdminService(HttpClient httpClient,
                            IAuthService authService,
                            ILogger<AdminService> logger)
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

            try
            {
                var response = await _httpClient.GetAsync($"admin/user-by-bp/{bp}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"GetUserByBp falló: {response.StatusCode}");
                    return null;
                }

                var user = await response.Content.ReadFromJsonAsync<AdminUser>();
                _logger.LogInformation($"Usuario encontrado: {user?.Name}");
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar usuario por BP");
                return null;
            }
        }

        public async Task<List<PendingOrderDto>> GetPendingOrdersAsync()
        {
            if (!await AttachTokenAsync())
                return new();

            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<PendingOrderDto>>(
                    "admin/pending-orders");

                return result ?? new List<PendingOrderDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener órdenes pendientes");
                return new List<PendingOrderDto>();
            }
        }

        public async Task<List<OrderDetailItemDto>> GetOrderDetailAsync(int orderId)
        {
            if (!await AttachTokenAsync())
                return new();

            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<OrderDetailItemDto>>(
                    $"admin/order-detail/{orderId}");

                return result ?? new List<OrderDetailItemDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener detalle de orden {orderId}");
                return new List<OrderDetailItemDto>();
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string accion)
        {
            if (!await AttachTokenAsync())
                return false;

            try
            {
                var request = new { OrderId = orderId, Accion = accion };

                var response = await _httpClient.PostAsJsonAsync(
                    "admin/update-order-status", request);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"UpdateOrderStatus falló: {response.StatusCode}");
                    return false;
                }

                var body = await response.Content.ReadFromJsonAsync<UpdateStatusResponse>();
                return body?.Success == true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar estado de orden {orderId}");
                return false;
            }
        }

        public async Task<LoginResponse?> LoginAsUserAsync(string bp)
        {
            if (!await AttachTokenAsync())
                return null;

            try
            {
                _logger.LogInformation($"Llamando a admin/login-as-user/{bp}");

                var response = await _httpClient.PostAsync($"admin/login-as-user/{bp}", null);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Login as user falló: {response.StatusCode}");
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning($"Error response: {errorContent}");
                    return null;
                }

                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                _logger.LogInformation($"Token obtenido para usuario: {loginResponse?.Name}");

                return loginResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al hacer login como usuario");
                return null;
            }
        }

        private sealed class UpdateStatusResponse
        {
            public bool Success { get; set; }
        }
    }
}