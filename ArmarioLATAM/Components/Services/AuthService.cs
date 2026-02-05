using ArmarioLATAM.Components.Models;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Server.Kestrel.Transport.NamedPipes;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;
using System.Xml.Linq;
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
        Task<DataUserSession?> GetSessionDataAsync();
        bool IsAuthenticated();
        Task<bool> SendResetEmailAsync(string email);
        Task<bool> ResetPasswordAsync(string token, string newPassword);
        /*Sesion Admin */
        Task<bool> SaveAdminSessionAsync();
        Task<bool> RestoreAdminSessionAsync();
        Task<LoginResponse?> LoginAsUserAsync(string bp, IAdminService adminService);
        Task<bool> HasAdminSessionAsync();
        Task<string?> GetActingAsUserNameAsync();
        Task<bool> IsActingAsUserAsync();
        Task InitializeSessionAsync();
        Task<AdminSessionInfo?> GetAdminSessionInfoAsync();
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthService> _logger;
        private readonly ProtectedSessionStorage _sessionStorage;
        private AuthSessionData _sessionData = new();

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

              
                // ✅ LOGGEAR EL TOKEN
                _logger.LogInformation("=== TOKEN GUARDADO ===");
                _logger.LogInformation($"Token: {_sessionData.Token}");
                _logger.LogInformation($"Expira en: {loginResponse.ExpiresIn} segundos");
                _logger.LogInformation($"Expira el: {_sessionData.TokenExpiration}");
                _logger.LogInformation($"Expira en: {loginResponse.ExpiresIn}");
                _logger.LogInformation($"Gender: {loginResponse.Gender}");
                _logger.LogInformation($"Role: {loginResponse.Role}");
                _logger.LogInformation($"ActivationDate: {loginResponse.ActivationDate}");
                _logger.LogInformation("=====================");

                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _sessionData.Token);

                // ✅ Guardar en sesión protegida
                await _sessionStorage.SetAsync("authSession", _sessionData);
            }

            return loginResponse;
        }

        // Inicializa Token/TokenExpiration desde sesión si están vacíos
        private async Task EnsureTokenLoadedAsync()
        {
            if (!string.IsNullOrWhiteSpace(_sessionData.Token))
                return;

            var stored = await _sessionStorage.GetAsync<AuthSessionData>("authSession");
           

            if (stored.Success && stored.Value is not null)
            {
                _sessionData = stored.Value;

                _logger.LogInformation("Token recuperado de sesión. Hash={Hash}", GetHashCode());

                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _sessionData.Token);
            }
            else
            {
                _logger.LogInformation("No hay token válido en sesión.");
            }
        }

        public async Task LogoutAsync()
        {
            _sessionData.Token = null;
            _sessionData.TokenExpiration = null;
            _sessionData.Name = "";
            _sessionData.BP = "";
            _httpClient.DefaultRequestHeaders.Authorization = null;

            // Limpiar sesión
            await _sessionStorage.DeleteAsync("authSession");
            await _sessionStorage.DeleteAsync("adminSession");

            await Task.CompletedTask;

            _logger.LogInformation("=== SESIÓN CERRADA ===");
            _logger.LogInformation("Token eliminado. Hash={Hash}", GetHashCode());
        }

        public string? GetToken() => _sessionData.Token;

        // Versión async para servicios que quieran forzar carga desde sesión
        public async Task<string?>GetTokenAsync()
        {
            if (string.IsNullOrWhiteSpace(_sessionData.Token))
            {
                await EnsureTokenLoadedAsync();
            }

            return _sessionData.Token;
        }

        // 3) Método ASYNC que combina ambos (para usar cuando puedas hacer await)
        public async Task<bool> IsSessionValidAsync()
        {
            await EnsureTokenLoadedAsync();

            if (string.IsNullOrEmpty(_sessionData.Token) || !_sessionData.TokenExpiration.HasValue)
                return false;

            return DateTime.UtcNow < _sessionData.TokenExpiration.Value;
        }

        // 2) Método SYNC que solo mira memoria (para layout, páginas, etc.)
        public bool IsAuthenticated()
        {
            if (string.IsNullOrEmpty(_sessionData.Token) || !_sessionData.TokenExpiration.HasValue)
                return false;

            return DateTime.UtcNow < _sessionData.TokenExpiration.Value;

        }
        /* 4) Nuevo método para obtener todos los datos de sesión */
        public async Task<DataUserSession?> GetSessionDataAsync()
        {
            var validSession = await IsSessionValidAsync();
            if (!validSession)
                return null;

            await EnsureTokenLoadedAsync();

            if (_sessionData == null)
                return null;

            return new DataUserSession
            {
                Name = _sessionData.Name,
                BP = _sessionData.BP,
                IsValid = true,
                Role = _sessionData.Role,
                Gender = _sessionData.Gender,
                BirthDate = _sessionData.BirthDate,
                ActivationDate = _sessionData.ActivationDate
            };
        }
        public async Task<bool> SendResetEmailAsync(string email)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("auth/forgot-password", new
                {
                    Email = email
                });

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando correo de recuperación");
                return false;
            }
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
                adminSession.Value.ActingAsUserBP = userData.BP;
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
        public async Task<AdminSessionInfo?> GetAdminSessionInfoAsync()
        {
            try
            {
                var stored = await _sessionStorage.GetAsync<AdminSessionData>("adminSession");

                if (!stored.Success || stored.Value is null)
                    return null;

                var adminData = stored.Value;

                return new AdminSessionInfo
                {
                    AdminName = adminData.AdminName,
                    AdminBP = adminData.AdminBP,
                    AdminToken = adminData.AdminToken,
                    ActingAsUserName = adminData.ActingAsUserName,
                    ActingAsUserBP = adminData.ActingAsUserBP,
                    TokenExpiration = adminData.TokenExpiration
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo información de sesión admin");
                return null;
            }
        }
        public async Task<string?> GetActingAsUserNameAsync()
        {
            try
            {
                var stored = await _sessionStorage.GetAsync<AdminSessionData>("adminSession");
                if (stored.Success && stored.Value != null)
                {
                    return stored.Value.ActingAsUserName;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo nombre de usuario 'actuando como'");
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
    }
}
