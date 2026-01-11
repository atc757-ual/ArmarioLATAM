using ArmarioLATAM.Components.Models;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace ArmarioLATAM.Services
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(string email, string password);
        Task LogoutAsync();
        string? GetToken();
        Task<string?> GetTokenAsync();
        Task<bool> IsSessionValidAsync();
        bool IsAuthenticated();

    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthService> _logger;
        private readonly ProtectedSessionStorage _sessionStorage;

        private string? _token;
        private DateTime? _tokenExpiration;

        public AuthService(HttpClient httpClient,
                           ILogger<AuthService> logger,
                           ProtectedSessionStorage sessionStorage)
        {
            _httpClient = httpClient;
            _logger = logger;
            _sessionStorage = sessionStorage;

            _logger.LogInformation("AuthService creado. Hash={Hash}", GetHashCode());
        }

        public async Task<LoginResponse?> LoginAsync(string email, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/login", new
            {
                Email = email,
                Password = password
            });

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"Login falló: {response.StatusCode}");
                return null;
            }

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

            if (!string.IsNullOrEmpty(loginResponse?.Token))
            {
                _token = loginResponse.Token;
                _tokenExpiration = DateTime.UtcNow.AddSeconds(loginResponse.ExpiresIn);

                // ✅ LOGGEAR EL TOKEN
                _logger.LogInformation("=== TOKEN GUARDADO ===");
                _logger.LogInformation($"Token: {_token}");
                _logger.LogInformation($"Expira en: {loginResponse.ExpiresIn} segundos");
                _logger.LogInformation($"Expira el: {_tokenExpiration}");
                _logger.LogInformation("=====================");

                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

                // ✅ Guardar en sesión protegida
                await _sessionStorage.SetAsync("authToken", _token);
                await _sessionStorage.SetAsync("authTokenExpiration", _tokenExpiration);
            }

            return loginResponse;
        }

        // Inicializa _token/_tokenExpiration desde sesión si están vacíos
        private async Task EnsureTokenLoadedAsync()
        {
            if (!string.IsNullOrWhiteSpace(_token))
                return;

            var storedToken = await _sessionStorage.GetAsync<string>("authToken");
            var storedExp = await _sessionStorage.GetAsync<DateTime?>("authTokenExpiration");

            if (storedToken.Success &&
                !string.IsNullOrWhiteSpace(storedToken.Value) &&
                storedExp.Success &&
                storedExp.Value.HasValue)
            {
                _token = storedToken.Value;
                _tokenExpiration = storedExp.Value.Value;

                _logger.LogInformation("Token recuperado de sesión. Hash={Hash}", GetHashCode());

                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);
            }
            else
            {
                _logger.LogInformation("No hay token válido en sesión.");
            }
        }

        public async Task LogoutAsync()
        {
            _token = null;
            _tokenExpiration = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;

            // Limpiar sesión
            await _sessionStorage.DeleteAsync("authToken");
            await _sessionStorage.DeleteAsync("authTokenExpiration");

            await Task.CompletedTask;
        }

        public string? GetToken() => _token;

        // Versión async para servicios que quieran forzar carga desde sesión
        public async Task<string?> GetTokenAsync()
        {
            if (string.IsNullOrWhiteSpace(_token))
            {
                await EnsureTokenLoadedAsync();
            }

            return _token;
        }

        // 3) Método ASYNC que combina ambos (para usar cuando puedas hacer await)
        public async Task<bool> IsSessionValidAsync()
        {
            await EnsureTokenLoadedAsync();

            if (string.IsNullOrEmpty(_token) || !_tokenExpiration.HasValue)
                return false;

            return DateTime.UtcNow < _tokenExpiration.Value;
        }

        // 2) Método SYNC que solo mira memoria (para layout, páginas, etc.)
        public bool IsAuthenticated()
        {
            if (string.IsNullOrEmpty(_token) || !_tokenExpiration.HasValue)
                return false;

            return DateTime.UtcNow < _tokenExpiration.Value;
        }


    }
}
