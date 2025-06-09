using Microsoft.JSInterop;
using System.Text.Json;
using static proyectoAgiles.Services.AuthService;

namespace proyectoAgiles.Services
{
    public class UserSessionService
    {
        private readonly IJSRuntime _jsRuntime;
        private UserDto? _currentUser;

        public UserSessionService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public UserDto? CurrentUser => _currentUser;

        public async Task InitializeAsync()
        {
            try
            {
                var userData = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "currentUser");
                if (!string.IsNullOrEmpty(userData))
                {
                    _currentUser = JsonSerializer.Deserialize<UserDto>(userData);
                }
            }
            catch (Exception)
            {
                // Si hay error al leer del localStorage, limpiar la sesión
                await ClearSessionAsync();
            }
        }

        public async Task SetUserAsync(UserDto user)
        {
            _currentUser = user;
            var userData = JsonSerializer.Serialize(user);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "currentUser", userData);
        }

        public async Task ClearSessionAsync()
        {
            _currentUser = null;
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "currentUser");
        }

        // Método sincrónico para usar en componentes
        public void ClearSession()
        {
            _currentUser = null;
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
        }
    }
}
