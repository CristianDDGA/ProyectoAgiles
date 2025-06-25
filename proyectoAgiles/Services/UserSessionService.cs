using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Text.Json;
using static proyectoAgiles.Services.AuthService;

namespace proyectoAgiles.Services
{    public class UserSessionService
    {
        private readonly IJSRuntime _jsRuntime;
        private UserDto? _currentUser;
        private bool _isInitialized = false;

        public UserSessionService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public UserDto? CurrentUser => _currentUser;        public async Task InitializeAsync()
        {
            if (_isInitialized) return;

            try
            {
                var userData = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "currentUser");
                if (!string.IsNullOrEmpty(userData))
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    };
                    _currentUser = JsonSerializer.Deserialize<UserDto>(userData, options);
                }
                _isInitialized = true;
            }
            catch (Exception)
            {
                // Si hay error al leer del localStorage, limpiar la sesión
                await ClearSessionAsync();
                _isInitialized = true;
            }
        }        public async Task SetUserAsync(UserDto user)
        {
            _currentUser = user;
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
            var userData = JsonSerializer.Serialize(user, options);
            Console.WriteLine($"Guardando usuario: {userData}"); // Debug temporal
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "currentUser", userData);
        }public async Task ClearSessionAsync()
        {
            _currentUser = null;
            _isInitialized = false;
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "currentUser");
        }        // Método sincrónico para usar en componentes
        public void ClearSession()
        {
            _currentUser = null;
            _isInitialized = false;
            // Note: Para operaciones síncronas, usaremos InvokeVoidAsync con el método async
            _ = Task.Run(async () => await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "currentUser"));
        }

        public bool IsAuthenticated => _currentUser != null;

        public bool IsAdmin => _currentUser?.UserType == 1;

        public bool IsTeacher => _currentUser?.UserType == 2;

        public string GetUserRole()
        {
            return _currentUser?.UserType switch
            {
                1 => "Administrador",
                2 => "Docente",
                _ => "Usuario"
            };
        }

        public string GetDashboardRoute()
        {
            return _currentUser?.UserType switch
            {
                1 => "/admin",
                2 => "/docente", 
                _ => "/"
            };
        }        public string GetUserNivel()
        {
            var nivel = _currentUser?.Nivel ?? string.Empty;
            Console.WriteLine($"GetUserNivel devuelve: '{nivel}'"); // Debug temporal
            return nivel;
        }

        public async Task RefreshUserDataAsync(HttpClient httpClient)
        {
            if (_currentUser?.Id == null) return;

            try
            {
                var response = await httpClient.GetAsync($"/api/users/{_currentUser.Id}");
                if (response.IsSuccessStatusCode)
                {
                    var updatedUser = await response.Content.ReadFromJsonAsync<UserDto>();
                    if (updatedUser != null)
                    {
                        await SetUserAsync(updatedUser);
                        Console.WriteLine($"Datos de usuario actualizados. Nuevo nivel: {updatedUser.Nivel}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al refrescar datos del usuario: {ex.Message}");
            }
        }
    }
}
