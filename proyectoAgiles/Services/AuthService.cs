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
                Cedula = request.Cedula,
                // Enviar datos del documento si existen
                IdentityDocument = request.DocumentFile,
                IdentityDocumentFileName = request.DocumentFileName,
                IdentityDocumentContentType = request.DocumentContentType
            };

            var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/api/auth/register", registerDto);
            
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error al registrar usuario: {errorContent}");
        }        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
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
                    Cedula = request.Cedula,
                    // Enviar datos del documento si existen
                    IdentityDocument = request.DocumentFile,
                    IdentityDocumentFileName = request.DocumentFileName,
                    IdentityDocumentContentType = request.DocumentContentType
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
        }        public async Task<LoginResponse> Login(LoginRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/api/auth/login", request);
                
                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    return loginResponse ?? new LoginResponse { Success = false, Message = "Error desconocido al procesar la respuesta." };
                }
                
                // Manejar errores estructurados del backend
                var errorContent = await response.Content.ReadAsStringAsync();
                
                try
                {
                    var errorJson = JsonSerializer.Deserialize<JsonElement>(errorContent);
                    
                    // Buscar el mensaje de error en diferentes propiedades
                    string errorMessage = "Error al iniciar sesión";
                    bool isAccountLocked = false;
                    
                    if (errorJson.TryGetProperty("details", out var detailsProperty))
                    {
                        var details = detailsProperty.GetString();
                        if (!string.IsNullOrEmpty(details))
                        {
                            errorMessage = details;
                            isAccountLocked = details.Contains("bloqueada") || details.Contains("locked");
                        }
                    }
                    else if (errorJson.TryGetProperty("message", out var messageProperty))
                    {
                        var message = messageProperty.GetString();
                        if (!string.IsNullOrEmpty(message))
                        {
                            errorMessage = message;
                            isAccountLocked = message.Contains("bloqueada") || message.Contains("locked");
                        }
                    }
                    
                    // Mejorar el mensaje para cuentas bloqueadas
                    if (isAccountLocked)
                    {
                        errorMessage = "🔒 Tu cuenta está temporalmente bloqueada por múltiples intentos fallidos de inicio de sesión.\n\n" +
                                     "Para desbloquear tu cuenta:\n" +
                                     "• Utiliza la opción \"¿Olvidaste tu contraseña?\" para restablecer tu contraseña\n" +
                                     "• O contacta al administrador del sistema\n\n" +
                                     "Tu cuenta se desbloqueará automáticamente después del restablecimiento de contraseña.";
                    }
                    
                    return new LoginResponse { Success = false, Message = errorMessage, IsAccountLocked = isAccountLocked };
                }
                catch
                {
                    // Si no se puede parsear el JSON, intentar manejar casos comunes
                    bool isAccountLocked = errorContent.Contains("bloqueada") || errorContent.Contains("locked");
                    
                    if (isAccountLocked)
                    {
                        errorContent = "🔒 Tu cuenta está temporalmente bloqueada por múltiples intentos fallidos de inicio de sesión.\n\n" +
                                     "Para desbloquear tu cuenta:\n" +
                                     "• Utiliza la opción \"¿Olvidaste tu contraseña?\" para restablecer tu contraseña\n" +
                                     "• O contacta al administrador del sistema\n\n" +
                                     "Tu cuenta se desbloqueará automáticamente después del restablecimiento de contraseña.";
                    }
                    
                    return new LoginResponse { Success = false, Message = errorContent, IsAccountLocked = isAccountLocked };
                }
            }
            catch (Exception ex)
            {
                return new LoginResponse { Success = false, Message = ex.Message };
            }
        }public async Task<bool> CheckEmailExists(string email)
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
        }        public async Task<ForgotPasswordResponse> ForgotPasswordAsync(string email)
        {
            try
            {
                var forgotPasswordDto = new { Email = email };
                var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/api/auth/forgot-password", forgotPasswordDto);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ForgotPasswordResponse>();
                    return result ?? new ForgotPasswordResponse { Success = false, Message = "Error desconocido" };
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    // Manejar respuestas BadRequest que contienen información del error
                    var errorResult = await response.Content.ReadFromJsonAsync<ForgotPasswordResponse>();
                    return errorResult ?? new ForgotPasswordResponse 
                    { 
                        Success = false, 
                        Message = "El correo electrónico no está registrado en nuestro sistema." 
                    };
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                return new ForgotPasswordResponse { Success = false, Message = $"Error: {errorContent}" };
            }
            catch (Exception ex)
            {
                return new ForgotPasswordResponse { Success = false, Message = ex.Message };
            }
        }

        public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordModel resetPasswordModel)
        {
            try
            {
                var resetPasswordDto = new
                {
                    Token = resetPasswordModel.Token,
                    Email = resetPasswordModel.Email,
                    NewPassword = resetPasswordModel.NewPassword,
                    ConfirmPassword = resetPasswordModel.ConfirmPassword
                };

                var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/api/auth/reset-password", resetPasswordDto);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ResetPasswordResponse>();
                    return result ?? new ResetPasswordResponse { Success = false, Message = "Error desconocido" };
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                return new ResetPasswordResponse { Success = false, Message = $"Error: {errorContent}" };
            }
            catch (Exception ex)
            {
                return new ResetPasswordResponse { Success = false, Message = ex.Message };
            }
        }
    }    public class RegisterRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
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
    }    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public UserDto? User { get; set; }
        public string Token { get; set; } = string.Empty;
        public bool IsAccountLocked { get; set; } = false;
    }public class UserDto
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
    }    public class ForgotPasswordResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El token es requerido")]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "La nueva contraseña es requerida")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        [MaxLength(100, ErrorMessage = "La contraseña no puede exceder 100 caracteres")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirma tu contraseña")]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }    public class ResetPasswordResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
