using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace proyectoAgiles.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public AuthService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5200";
        }        public async Task<bool> Register(RegisterRequest request)
        {
            // Mapear RegisterRequest a RegisterDto (formato del backend)
            var registerDto = new
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = request.Password,
                ConfirmPassword = request.ConfirmPassword,
                Cedula = request.Cedula
            };

            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/api/auth/register", registerDto);
            
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error al registrar usuario: {errorContent}");
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {                // Mapear RegisterRequest a RegisterDto (formato del backend)
                var registerDto = new
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Password = request.Password,
                    ConfirmPassword = request.ConfirmPassword,
                    Cedula = request.Cedula
                };

                var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/api/auth/register", registerDto);
                  if (response.IsSuccessStatusCode)
                {
                    return new RegisterResponse { IsSuccess = true, Message = "¡Registro exitoso!" };
                }
                
                // Intentar parsear el error como JSON para obtener el mensaje específico
                try
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorJson = JsonSerializer.Deserialize<JsonElement>(errorContent);
                    
                    string errorMessage = "Error durante el registro";
                    
                    // Intentar extraer el mensaje de error del JSON
                    if (errorJson.TryGetProperty("message", out var messageProperty))
                    {
                        errorMessage = messageProperty.GetString() ?? errorMessage;
                    }
                    else if (errorJson.TryGetProperty("Message", out var messageProperty2))
                    {
                        errorMessage = messageProperty2.GetString() ?? errorMessage;
                    }
                    
                    return new RegisterResponse { IsSuccess = false, ErrorMessage = errorMessage };
                }
                catch
                {
                    // Si no se puede parsear el JSON, usar el contenido completo
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new RegisterResponse { IsSuccess = false, ErrorMessage = !string.IsNullOrEmpty(errorContent) ? errorContent : "Error durante el registro" };
                }
            }
            catch (Exception ex)
            {
                return new RegisterResponse { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }        public async Task<LoginResponse?> Login(LoginRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/api/auth/login", request);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<LoginResponse>();
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error al iniciar sesión: {errorContent}");
        }        public async Task<bool> CheckEmailExists(string email)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/auth/check-email/{email}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CheckEmailResponse>();
                return result?.exists ?? false;
            }
            return false;
        }

        public async Task<bool> CheckCedulaExists(string cedula)
        {
            var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/auth/check-cedula/{cedula}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<CheckEmailResponse>();
                return result?.exists ?? false;
            }
            return false;
        }
    }public class RegisterRequest
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        [MaxLength(255, ErrorMessage = "El email no puede exceder 255 caracteres")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        [MaxLength(100, ErrorMessage = "La contraseña no puede exceder 100 caracteres")]
        public string Password { get; set; } = string.Empty;        [Required(ErrorMessage = "Confirma tu contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "La cédula es requerida")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "La cédula debe tener exactamente 10 dígitos")]
        [MaxLength(10, ErrorMessage = "La cédula debe tener 10 dígitos")]
        public string Cedula { get; set; } = string.Empty;

        // Propiedades para el archivo de documento
        public byte[]? DocumentFile { get; set; }
        public string? DocumentFileName { get; set; }
        public string? DocumentContentType { get; set; }

        // Propiedades originales para compatibilidad con el backend
        public string FirstName => GetFirstName();
        public string LastName => GetLastName();

        private string GetFirstName()
        {
            if (string.IsNullOrEmpty(Name)) return string.Empty;
            var parts = Name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 0 ? parts[0] : string.Empty;
        }

        private string GetLastName()
        {
            if (string.IsNullOrEmpty(Name)) return string.Empty;
            var parts = Name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : string.Empty;
        }
    }public class LoginRequest
    {
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        public string Password { get; set; } = string.Empty;
    }    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public UserDto? User { get; set; }
        public string Token { get; set; } = string.Empty;
    }    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int UserType { get; set; }
        public string Cedula { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }    public class CheckEmailResponse
    {
        public bool exists { get; set; }
    }

    public class RegisterResponse
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
