using ArmarioLATAM.Components.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ArmarioLATAM.Services
{
    public interface IOrderService
    {
        Task<HttpResponseMessage?> CreateOrderAsync(CreateOrder dto);
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
    }
}
