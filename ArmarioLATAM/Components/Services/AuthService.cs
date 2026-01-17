using ArmarioLATAM.Components.Models;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

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

        Task<DataUserSession?> GetSessionDataAsync();

        Task<bool> SendResetEmailAsync(string email);
        Task<bool> ResetPasswordAsync(string token, string newPassword);

        Task<bool> SaveAdminSessionAsync();
        Task<bool> RestoreAdminSessionAsync();
        Task<LoginResponse?> LoginAsUserAsync(string bp, IAdminService adminService);

        Task<bool> HasAdminSessionAsync();
        Task<string> GetActingAsUserNameAsync();

        // ✅ NUEVOS
        Task<bool> IsActingAsUserAsync();
        Task InitializeSessionAsync();
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthService> _logger;
        private readonly ProtectedSessionStorage _sessionStorage;

        private AuthSessionData _sessionData = new();

        public AuthService(
            HttpClient httpClient,
            ILogger<AuthService> logger,
            ProtectedSessionStorage sessionStorage)
        {
            _httpClient = httpClient;
            _logger = logger;
            _sessionStorage = sessionStorage;

            _logger.LogInformation("AuthService creado. Hash={Hash}", GetHashCode());
        }

        // =========================
        // LOGIN NORMAL
        // =========================
        public async Task<LoginResponse?> LoginAsync(string email, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/login", new
            {
                Email = email,
                Password = password
            });

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Login falló: {Status}", response.StatusCode);
                return null;
            }

            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();

            if (string.IsNullOrEmpty(loginResponse?.Token))
                return null;

            _sessionData = new AuthSessionData
            {
                Token = loginResponse.Token,
                Name = loginResponse.Name,
                BP = loginResponse.BP,
                Role = loginResponse.Role,
                Gender = loginResponse.Gender,
                ActivationDate = loginResponse.ActivationDate,
                BirthDate = loginResponse.BirthDate,
                TokenExpiration = DateTime.UtcNow.AddSeconds(loginResponse.ExpiresIn)
            };

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _sessionData.Token);

            await _sessionStorage.SetAsync("authSession", _sessionData);

            _logger.LogInformation("Login exitoso. Usuario={Name}, Role={Role}", _sessionData.Name, _sessionData.Role);

            return loginResponse;
        }

        // =========================
        // INICIALIZACIÓN DE SESIÓN
        // =========================
        public async Task InitializeSessionAsync()
        {
            _logger.LogInformation("=== INICIALIZANDO SESIÓN ===");

            if (await HasAdminSessionAsync())
            {
                _logger.LogInformation("Usuario está actuando como otro (sesión admin pendiente)");
                return;
            }

            await EnsureTokenLoadedAsync();

            _logger.LogInformation("Token cargado: {HasToken}", !string.IsNullOrEmpty(_sessionData.Token));
            _logger.LogInformation("===========================");
        }

        private async Task EnsureTokenLoadedAsync()
        {
            if (!string.IsNullOrWhiteSpace(_sessionData.Token))
                return;

            var stored = await _sessionStorage.GetAsync<AuthSessionData>("authSession");

            if (!stored.Success || stored.Value is null)
            {
                _logger.LogInformation("No hay sesión válida en storage");
                return;
            }

            _sessionData = stored.Value;

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _sessionData.Token);

            _logger.LogInformation("Token recuperado desde sesión");
        }

        // =========================
        // ESTADO DE SESIÓN
        // =========================
        public string? GetToken() => _sessionData.Token;

        public async Task<string?> GetTokenAsync()
        {
            await EnsureTokenLoadedAsync();
            return _sessionData.Token;
        }

        public async Task<bool> IsSessionValidAsync()
        {
            await EnsureTokenLoadedAsync();

            if (string.IsNullOrEmpty(_sessionData.Token) || !_sessionData.TokenExpiration.HasValue)
                return false;

            return DateTime.UtcNow < _sessionData.TokenExpiration.Value;
        }

        public bool IsAuthenticated()
        {
            if (string.IsNullOrEmpty(_sessionData.Token) || !_sessionData.TokenExpiration.HasValue)
                return false;

            return DateTime.UtcNow < _sessionData.TokenExpiration.Value;
        }

        public async Task<DataUserSession?> GetSessionDataAsync()
        {
            if (!await IsSessionValidAsync())
                return null;

            return new DataUserSession
            {
                Name = _sessionData.Name,
                BP = _sessionData.BP,
                Role = _sessionData.Role,
                Gender = _sessionData.Gender,
                BirthDate = _sessionData.BirthDate,
                ActivationDate = _sessionData.ActivationDate,
                IsValid = true
            };
        }

        // =========================
        // ADMIN → USUARIO
        // =========================
        public async Task<bool> SaveAdminSessionAsync()
        {
            if (string.IsNullOrWhiteSpace(_sessionData.Token) || _sessionData.Role != "admin")
                return false;

            var adminSession = new AdminSessionData
            {
                AdminToken = _sessionData.Token,
                AdminName = _sessionData.Name,
                AdminBP = _sessionData.BP,
                TokenExpiration = _sessionData.TokenExpiration
            };

            await _sessionStorage.SetAsync("adminSession", adminSession);

            _logger.LogInformation("Sesión admin guardada");
            return true;
        }

        public async Task<LoginResponse?> LoginAsUserAsync(string bp, IAdminService adminService)
        {
            _logger.LogInformation("=== INICIANDO LOGIN COMO USUARIO ===");
            _logger.LogInformation($"BP del usuario: {bp}");

            // Primero obtener información del usuario
            var userData = await adminService.GetUserByBpAsync(bp);
            if (userData == null)
            {
                _logger.LogError("Usuario no encontrado");
                return null;
            }

            // Guardar sesión del admin
            var saved = await SaveAdminSessionAsync();
            if (!saved)
            {
                _logger.LogError("No se pudo guardar la sesión del admin");
                return null;
            }

            // Guardar nombre del usuario en la sesión de admin
            var adminSession = await _sessionStorage.GetAsync<AdminSessionData>("adminSession");
            if (adminSession.Success && adminSession.Value != null)
            {
                adminSession.Value.ActingAsUserName = userData.Name;
                await _sessionStorage.SetAsync("adminSession", adminSession.Value);
            }

            // Obtener token del usuario
            var userLoginResponse = await adminService.LoginAsUserAsync(bp);

            if (userLoginResponse == null || string.IsNullOrEmpty(userLoginResponse.Token))
            {
                _logger.LogWarning("No se pudo obtener token del usuario");
                // Restaurar sesión del admin si falló
                await RestoreAdminSessionAsync();
                return null;
            }

            // Actualizar sesión con datos del usuario
            _sessionData = new AuthSessionData
            {
                Token = userLoginResponse.Token,
                Name = userLoginResponse.Name,
                BP = userLoginResponse.BP,
                Role = userLoginResponse.Role,
                Gender = userLoginResponse.Gender,
                ActivationDate = userLoginResponse.ActivationDate,
                BirthDate = userLoginResponse.BirthDate,
                TokenExpiration = DateTime.UtcNow.AddSeconds(userLoginResponse.ExpiresIn)
            };

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _sessionData.Token);

            await _sessionStorage.SetAsync("authSession", _sessionData);

            _logger.LogInformation("=== LOGIN COMO USUARIO EXITOSO ===");
            _logger.LogInformation($"Usuario: {userLoginResponse.Name}");
            _logger.LogInformation($"BP: {userLoginResponse.BP}");
            _logger.LogInformation($"Role: {userLoginResponse.Role}");
            _logger.LogInformation("===================================");

            return userLoginResponse;
        }
        public async Task<string?> GetActingAsUserNameAsync()
        {
            var stored = await _sessionStorage.GetAsync<AdminSessionData>("adminSession");
            if (stored.Success && stored.Value != null)
            {
                return stored.Value.ActingAsUserName;
            }
            return null;
        }

        public async Task<bool> RestoreAdminSessionAsync()
        {
            var stored = await _sessionStorage.GetAsync<AdminSessionData>("adminSession");

            if (!stored.Success || stored.Value is null)
                return false;

            var admin = stored.Value;

            _sessionData = new AuthSessionData
            {
                Token = admin.AdminToken!,
                Name = admin.AdminName!,
                BP = admin.AdminBP!,
                Role = "admin",
                TokenExpiration = admin.TokenExpiration
            };

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _sessionData.Token);

            await _sessionStorage.DeleteAsync("adminSession");
            await _sessionStorage.SetAsync("authSession", _sessionData);

            _logger.LogInformation("Sesión admin restaurada");
            return true;
        }

        // =========================
        // DETECCIÓN DE ESTADO
        // =========================
        public async Task<bool> HasAdminSessionAsync()
        {
            try
            {
                var stored = await _sessionStorage.GetAsync<AdminSessionData>("adminSession");

                if (!stored.Success || stored.Value is null)
                    return false;

                var adminData = stored.Value;

                if (string.IsNullOrWhiteSpace(adminData.AdminToken))
                    return false;

                if (adminData.TokenExpiration.HasValue &&
                    DateTime.UtcNow >= adminData.TokenExpiration.Value)
                {
                    await _sessionStorage.DeleteAsync("adminSession");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verificando sesión de admin");
                return false;
            }
        }

        public async Task<bool> IsActingAsUserAsync()
        {
            return await HasAdminSessionAsync();
        }

        // =========================
        // LOGOUT
        // =========================
        public async Task LogoutAsync()
        {
            _sessionData = new AuthSessionData();
            _httpClient.DefaultRequestHeaders.Authorization = null;

            await _sessionStorage.DeleteAsync("authSession");
            await _sessionStorage.DeleteAsync("adminSession");

            _logger.LogInformation("Sesión cerrada completamente");
        }

        // =========================
        // PASSWORD
        // =========================
        public async Task<bool> SendResetEmailAsync(string email)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/forgot-password", new { Email = email });
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            var response = await _httpClient.PostAsJsonAsync("auth/reset-password", new
            {
                Token = token,
                NewPassword = newPassword
            });

            return response.IsSuccessStatusCode;
        }
    }
}
