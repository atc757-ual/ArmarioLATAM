using ArmarioLATAM.Components.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ArmarioLATAM.Services
{
    public interface IOrderService
    {
        Task<HttpResponseMessage?> CreateOrderAsync(CreateOrder dto);
        Task<MyOrdersResponse?> GetMyOrdersAsync();
        Task<OrderDetailResponse?> GetOrderByIdAsync(int id);
        Task<MyOrdersResponse?> GetMyOrdersHistoryAsync();
        Task<PendingOrderResponse?> GetMyPendingOrderAsync(int? kitTypeId = null);
    }

    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(HttpClient httpClient,
                            IAuthService authService,
                            ILogger<OrderService> logger)
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
                _logger.LogWarning("Sin token válido, NO se llama al API (Orders).");
                return false;
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            return true;
        }

        public async Task<HttpResponseMessage?> CreateOrderAsync(CreateOrder dto)
        {
            if (!await AttachTokenAsync())
                return null;

            var response = await _httpClient.PostAsJsonAsync("api/Orders", dto);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                _logger.LogWarning("CreateOrderAsync -> Unauthorized");
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error al crear orden. Status={Status}, Body={Body}",
                                 response.StatusCode, body);
            }

            return response;
        }

        public async Task<MyOrdersResponse?> GetMyOrdersAsync()
        {
            if (!await AttachTokenAsync())
                return null;

            var response = await _httpClient.GetAsync("api/Orders/my-orders");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _logger.LogWarning("GetMyOrdersAsync -> Unauthorized");
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error al obtener órdenes. Status={Status}, Body={Body}",
                                 response.StatusCode, body);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<MyOrdersResponse>();
        }


        public async Task<OrderDetailResponse?> GetOrderByIdAsync(int id)
        {
            if (!await AttachTokenAsync())
                return null;

            var response = await _httpClient.GetAsync($"api/Orders/{id}");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _logger.LogWarning("GetOrderByIdAsync -> Unauthorized");
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error al obtener orden. Status={Status}, Body={Body}",
                                 response.StatusCode, body);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<OrderDetailResponse>();
        }

        public async Task<MyOrdersResponse?> GetMyOrdersHistoryAsync()
        {
            if (!await AttachTokenAsync())
                return null;

            var response = await _httpClient.GetAsync("api/Orders/my-orders-history");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return null;

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<MyOrdersResponse>();
        }

        public async Task<PendingOrderResponse?> GetMyPendingOrderAsync(int? kitTypeId = null)
        {
            if (!await AttachTokenAsync())
                return null;

            var url = "api/Orders/my-order-pending";
            if (kitTypeId.HasValue)
            {
                url += $"?kitTypeId={kitTypeId.Value}";
            }

            var response = await _httpClient.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return null;

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<PendingOrderResponse>();
        }
    }
}
