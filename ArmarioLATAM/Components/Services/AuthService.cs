using ArmarioLATAM.Components.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace ArmarioLATAM.Services
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(string email, string password);
        Task LogoutAsync();
        string? GetToken();
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthService> _logger;
        private string? _token;

        public AuthService(HttpClient httpClient, ILogger<AuthService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

       public async Task<LoginResponse?> LoginAsync(string email, string password)
{
    try
    {
        var request = new LoginRequest
        {
            Email = email,
            Password = password
        };

        // 👇 Ver qué URL está usando
        var url = $"{_httpClient.BaseAddress}auth/login";
        _logger.LogInformation($"Intentando login en: {url}");
        _logger.LogInformation($"Email: {email}");

        var response = await _httpClient.PostAsJsonAsync("auth/login", request);

        // 👇 Ver el status code
        _logger.LogInformation($"Status Code: {response.StatusCode}");

        if (response.IsSuccessStatusCode)
        {
            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            
            if (loginResponse != null)
            {
                _token = loginResponse.Token;
                _logger.LogInformation("Login exitoso!");
            }

            return loginResponse;
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning($"Login failed: {response.StatusCode} - {errorContent}");
            return null;
        }
    }
    catch (HttpRequestException ex)
    {
        _logger.LogError(ex, $"Error de red: {ex.Message}");
        return null;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error durante el login");
        return null;
    }
}

        public Task LogoutAsync()
        {
            _token = null;
            return Task.CompletedTask;
        }

        public string? GetToken()
        {
            return _token;
        }
    }
}
