using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Configuration;
using ProyectoAgiles.Application.DTOs;

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
        }

        public async Task<bool> Register(RegisterRequest request)
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
        }        public async Task<bool> SubirNivel(string cedula)
        {
            try
            {
                // Primero verificar si cumple todos los requisitos
                var verificacion = await VerificarRequisitosSubirNivel(cedula);
                
                if (!verificacion.CumpleTodosRequisitos)
                {
                    throw new Exception($"No cumple con los requisitos para subir de nivel: {verificacion.Mensaje}");
                }

                // Si cumple todos los requisitos, proceder con el cambio de nivel
                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/users/by-cedula/{cedula}/subir-nivel", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al subir de nivel: {ex.Message}");
            }
        }public async Task<VerificacionRequisitosSubirNivelDto> VerificarRequisitosSubirNivel(string cedula)
        {
            try
            {
                var verificacion = new VerificacionRequisitosSubirNivelDto
                {
                    Cedula = cedula,
                    CumpleTodosRequisitos = false,
                    Mensaje = "Verificando requisitos para subir de nivel..."
                };

                // 1. Verificar experiencia mínima de 4 años
                verificacion.Experiencia = await VerificarExperienciaMinima(cedula);

                // 2. Verificar obra relevante o artículo indexado con filiación UTA
                verificacion.ObraRelevante = await VerificarObraRelevante(cedula);

                // 3. Verificar evaluación 75% en últimos 4 períodos
                verificacion.Evaluacion75Porciento = await VerificarEvaluacion75Porciento(cedula);

                // 4. Verificar 96 horas de capacitación (24 horas pedagógicas)
                verificacion.Capacitacion96Horas = await VerificarCapacitacion96Horas(cedula);

                // Determinar si cumple todos los requisitos
                verificacion.CumpleTodosRequisitos = 
                    verificacion.Experiencia.Cumple &&
                    verificacion.ObraRelevante.Cumple &&
                    verificacion.Evaluacion75Porciento.Cumple &&
                    verificacion.Capacitacion96Horas.Cumple;

                // Generar mensaje final
                if (verificacion.CumpleTodosRequisitos)
                {
                    verificacion.Mensaje = "✅ CUMPLE con todos los requisitos para subir de nivel a Titular Auxiliar 2";
                }
                else
                {
                    var requisitosIncumplidos = new List<string>();
                    if (!verificacion.Experiencia.Cumple) requisitosIncumplidos.Add("Experiencia mínima");
                    if (!verificacion.ObraRelevante.Cumple) requisitosIncumplidos.Add("Obra relevante/artículo indexado");
                    if (!verificacion.Evaluacion75Porciento.Cumple) requisitosIncumplidos.Add("Evaluación 75%");
                    if (!verificacion.Capacitacion96Horas.Cumple) requisitosIncumplidos.Add("Capacitación 96 horas");
                    
                    verificacion.Mensaje = $"❌ NO CUMPLE con los siguientes requisitos: {string.Join(", ", requisitosIncumplidos)}";
                }

                return verificacion;
            }
            catch (Exception ex)
            {
                return new VerificacionRequisitosSubirNivelDto
                {
                    Cedula = cedula,
                    CumpleTodosRequisitos = false,
                    Mensaje = $"Error al verificar requisitos: {ex.Message}"
                };
            }
        }
          private async Task<RequisitoCumplimientoDto> VerificarExperienciaMinima(string cedula)
        {
            try
            {                // Obtener información de TTHH para verificar fecha de ingreso como titular auxiliar 1
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/tthh/by-cedula/{cedula}");
                if (response.IsSuccessStatusCode)
                {
                    var tthhInfo = await response.Content.ReadFromJsonAsync<TTHHDto>();
                    if (tthhInfo != null)
                    {
                        // Calcular años desde la fecha de inicio registrada en TTHH
                        var añosExperiencia = (DateTime.Now - tthhInfo.FechaInicio).TotalDays / 365.25;
                        
                        return new RequisitoCumplimientoDto
                        {
                            Cumple = añosExperiencia >= 4,
                            Mensaje = $"Experiencia: {añosExperiencia:F1} años como titular auxiliar 1 desde {tthhInfo.FechaInicio:dd/MM/yyyy} " +
                                     (añosExperiencia >= 4 ? "(✅ Cumple - mínimo 4 años)" : "(❌ No cumple - requiere mínimo 4 años)"),
                            ValorObtenido = $"{añosExperiencia:F1} años desde {tthhInfo.FechaInicio:dd/MM/yyyy}",
                            ValorRequerido = "4 años mínimo como titular auxiliar 1"
                        };
                    }
                }
                
                // Fallback: Si no existe TTHH, usar fecha de creación del usuario
                var userResponse = await _httpClient.GetAsync($"{_apiBaseUrl}/api/users/by-cedula/{cedula}");
                if (userResponse.IsSuccessStatusCode)
                {
                    var userInfo = await userResponse.Content.ReadFromJsonAsync<UserDto>();
                    if (userInfo != null)
                    {
                        var añosExperiencia = (DateTime.Now - userInfo.CreatedAt).TotalDays / 365.25;
                        
                        return new RequisitoCumplimientoDto
                        {
                            Cumple = añosExperiencia >= 4,
                            Mensaje = $"Experiencia (estimada): {añosExperiencia:F1} años desde registro {userInfo.CreatedAt:dd/MM/yyyy} " +
                                     (añosExperiencia >= 4 ? "(✅ Cumple - mínimo 4 años)" : "(❌ No cumple - requiere mínimo 4 años)") +
                                     " (Datos TTHH no disponibles)",
                            ValorObtenido = $"{añosExperiencia:F1} años (estimado)",
                            ValorRequerido = "4 años mínimo como titular auxiliar 1"
                        };
                    }
                }
                
                return new RequisitoCumplimientoDto
                {
                    Cumple = false,
                    Mensaje = "❌ No se pudo verificar la experiencia mínima - Sin datos TTHH ni información de usuario",
                    ValorObtenido = "No disponible",
                    ValorRequerido = "4 años mínimo como titular auxiliar 1"
                };
            }
            catch (Exception ex)
            {
                return new RequisitoCumplimientoDto
                {
                    Cumple = false,
                    Mensaje = $"❌ Error al verificar experiencia: {ex.Message}",
                    ValorObtenido = "Error",
                    ValorRequerido = "4 años mínimo como titular auxiliar 1"
                };
            }
        }

        private async Task<RequisitoCumplimientoDto> VerificarObraRelevante(string cedula)
        {
            try
            {
                var investigaciones = await GetInvestigacionesDisponiblesPorCedula(cedula);
                
                // Buscar investigaciones con filiación UTA
                var investigacionesUTA = investigaciones.Where(i => 
                    i.Filiacion.Contains("UTA", StringComparison.OrdinalIgnoreCase) ||
                    i.Filiacion.Contains("Universidad Técnica de Ambato", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (investigacionesUTA.Any())
                {
                    return new RequisitoCumplimientoDto
                    {
                        Cumple = true,
                        Mensaje = $"✅ Tiene {investigacionesUTA.Count} obra(s) relevante(s) con filiación UTA",
                        ValorObtenido = $"{investigacionesUTA.Count} investigación(es) con filiación UTA",
                        ValorRequerido = "Al menos 1 obra relevante con filiación UTA"
                    };
                }
                else
                {
                    return new RequisitoCumplimientoDto
                    {
                        Cumple = false,
                        Mensaje = $"❌ No tiene obras relevantes con filiación UTA (Total: {investigaciones.Count})",
                        ValorObtenido = $"{investigaciones.Count} investigación(es) sin filiación UTA",
                        ValorRequerido = "Al menos 1 obra relevante con filiación UTA"
                    };
                }
            }
            catch (Exception ex)
            {
                return new RequisitoCumplimientoDto
                {
                    Cumple = false,
                    Mensaje = $"❌ Error al verificar obra relevante: {ex.Message}",
                    ValorObtenido = "Error",
                    ValorRequerido = "Al menos 1 obra relevante con filiación UTA"
                };
            }
        }

        private async Task<RequisitoCumplimientoDto> VerificarEvaluacion75Porciento(string cedula)
        {
            try
            {                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/EvaluacionesDesempeno/verificar-requisito-75/{cedula}");
                if (response.IsSuccessStatusCode)
                {
                    var verificacion = await response.Content.ReadFromJsonAsync<VerificacionRequisito75Dto>();
                    if (verificacion != null)
                    {
                        return new RequisitoCumplimientoDto
                        {
                            Cumple = verificacion.CumpleRequisito,
                            Mensaje = verificacion.CumpleRequisito 
                                ? $"✅ Cumple evaluación 75%: {verificacion.PorcentajePromedioUltimasCuatro:F1}% promedio en últimos {verificacion.EvaluacionesAnalizadas} períodos"
                                : $"❌ No cumple evaluación 75%: {verificacion.PorcentajePromedioUltimasCuatro:F1}% promedio en últimos {verificacion.EvaluacionesAnalizadas} períodos",
                            ValorObtenido = $"{verificacion.PorcentajePromedioUltimasCuatro:F1}% promedio",
                            ValorRequerido = "75% mínimo en últimos 4 períodos"
                        };
                    }
                }
                
                return new RequisitoCumplimientoDto
                {
                    Cumple = false,
                    Mensaje = "❌ No se pudo verificar las evaluaciones de desempeño",
                    ValorObtenido = "No disponible",
                    ValorRequerido = "75% mínimo en últimos 4 períodos"
                };
            }
            catch (Exception ex)
            {
                return new RequisitoCumplimientoDto
                {
                    Cumple = false,
                    Mensaje = $"❌ Error al verificar evaluación: {ex.Message}",
                    ValorObtenido = "Error",
                    ValorRequerido = "75% mínimo en últimos 4 períodos"
                };
            }
        }        private async Task<RequisitoCumplimientoDto> VerificarCapacitacion96Horas(string cedula)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/ditic/verificar-requisito/{cedula}");
                if (response.IsSuccessStatusCode)
                {
                    var verificacion = await response.Content.ReadFromJsonAsync<VerificacionRequisitoDiticResponse>();
                    if (verificacion != null)
                    {
                        var cumpleHoras = verificacion.HorasObtenidas >= 96;
                        var cumplePedagogico = verificacion.HorasPedagogicasObtenidas >= 24;
                        var cumpleRequisito = (cumpleHoras && cumplePedagogico) || verificacion.TieneExencionAutoridad;

                        string mensaje;
                        if (verificacion.TieneExencionAutoridad)
                        {
                            mensaje = $"✅ Exento por autoridad: {verificacion.CargoAutoridad} ({verificacion.AñosComoAutoridad:F1} años)";
                        }
                        else if (cumpleRequisito)
                        {
                            mensaje = $"✅ Cumple capacitación: {verificacion.HorasObtenidas}h totales ({verificacion.HorasPedagogicasObtenidas}h pedagógicas)";
                        }
                        else
                        {
                            mensaje = $"❌ No cumple capacitación: {verificacion.HorasObtenidas}h totales ({verificacion.HorasPedagogicasObtenidas}h pedagógicas)";
                        }

                        return new RequisitoCumplimientoDto
                        {
                            Cumple = cumpleRequisito,
                            Mensaje = mensaje,
                            ValorObtenido = verificacion.TieneExencionAutoridad 
                                ? $"Exento - {verificacion.CargoAutoridad}"
                                : $"{verificacion.HorasObtenidas}h ({verificacion.HorasPedagogicasObtenidas}h pedagógicas)",
                            ValorRequerido = "96h totales (24h pedagógicas mín.) en últimos 3 años"
                        };
                    }
                }
                
                return new RequisitoCumplimientoDto
                {
                    Cumple = false,
                    Mensaje = "❌ No se pudo verificar las capacitaciones DITIC",
                    ValorObtenido = "No disponible",
                    ValorRequerido = "96h totales (24h pedagógicas mín.) en últimos 3 años"
                };
            }
            catch (Exception ex)
            {
                return new RequisitoCumplimientoDto
                {
                    Cumple = false,
                    Mensaje = $"❌ Error al verificar capacitación DITIC: {ex.Message}",
                    ValorObtenido = "Error",
                    ValorRequerido = "96h totales (24h pedagógicas mín.) en últimos 3 años"
                };
            }
        }

        // Métodos para trabajar con investigaciones
        
        public async Task<List<InvestigacionDto>> GetInvestigacionesPorCedula(string cedula)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<InvestigacionDto>>($"{_apiBaseUrl}/api/investigaciones/by-cedula/{cedula}");
                return response ?? new List<InvestigacionDto>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener investigaciones: {ex.Message}");
            }
        }

        public async Task<List<InvestigacionDto>> GetInvestigacionesDisponiblesPorCedula(string cedula)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<InvestigacionDto>>($"{_apiBaseUrl}/api/investigaciones/disponibles/{cedula}");
                return response ?? new List<InvestigacionDto>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener investigaciones disponibles: {ex.Message}");
            }
        }

        public async Task<EstadisticasDocenteResponse> ObtenerEstadisticasDocente(string cedula)
        {
            try
            {
                Console.WriteLine($"ObtenerEstadisticasDocente: Solicitando estadísticas para cédula {cedula}");
                
                // Usar la nueva lógica dinámica en lugar de la API antigua
                var estadisticasDinamicas = await ObtenerEstadisticasDocenteDinamicas(cedula);
                if (estadisticasDinamicas != null)
                {
                    Console.WriteLine($"ObtenerEstadisticasDocente: Usando estadísticas dinámicas");
                    return estadisticasDinamicas;
                }
                
                // Fallback a la API antigua si falla la lógica dinámica
                var url = $"{_apiBaseUrl}/api/EvaluacionesDesempeno/estadisticas-docente/{cedula}";
                Console.WriteLine($"ObtenerEstadisticasDocente: Fallback a URL = {url}");
                
                var response = await _httpClient.GetAsync(url);
                Console.WriteLine($"ObtenerEstadisticasDocente: Status = {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"ObtenerEstadisticasDocente: JSON Response = {json}");
                    
                    var estadisticas = JsonSerializer.Deserialize<EstadisticasDocenteResponse>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    Console.WriteLine($"ObtenerEstadisticasDocente: Estadísticas deserializadas exitosamente");
                    return estadisticas ?? CreateDefaultEstadisticas(cedula);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"ObtenerEstadisticasDocente: Error {response.StatusCode} - {errorContent}");
                    return CreateDefaultEstadisticas(cedula);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ObtenerEstadisticasDocente: Exception - {ex.Message}");
                return CreateDefaultEstadisticas(cedula);
            }
        }

        private async Task<EstadisticasDocenteResponse?> ObtenerEstadisticasDocenteDinamicas(string cedula)
        {
            try
            {
                Console.WriteLine($"ObtenerEstadisticasDocenteDinamicas: Iniciando para cédula {cedula}");
                
                // Obtener información del usuario para saber su nivel actual
                var usuario = await ObtenerDatosUsuarioSession(cedula);
                if (usuario == null || string.IsNullOrEmpty(usuario.Nivel))
                {
                    Console.WriteLine($"ObtenerEstadisticasDocenteDinamicas: No se pudo obtener el nivel del usuario");
                    return null;
                }

                var nivelActual = usuario.Nivel;
                Console.WriteLine($"ObtenerEstadisticasDocenteDinamicas: Nivel actual = {nivelActual}");
                
                // Obtener configuración de requisitos
                var requisitosService = new ProyectoAgiles.Application.Services.RequisitosEscalafonService();
                var configuracion = requisitosService.GetRequisitosParaNivel(nivelActual);
                
                if (configuracion == null)
                {
                    Console.WriteLine($"ObtenerEstadisticasDocenteDinamicas: No se encontró configuración para nivel {nivelActual}");
                    return CreateDefaultEstadisticas(cedula);
                }

                // Verificar requisitos usando la lógica dinámica
                var verificacion = await VerificarRequisitosEscalafonDinamico(cedula, nivelActual);
                
                if (verificacion == null)
                {
                    Console.WriteLine($"ObtenerEstadisticasDocenteDinamicas: No se pudo verificar requisitos");
                    return CreateDefaultEstadisticas(cedula);
                }

                // Crear estadísticas basadas en la verificación dinámica
                var estadisticas = await CrearEstadisticasDesdeVerificacionDinamica(cedula, verificacion, configuracion);
                
                Console.WriteLine($"ObtenerEstadisticasDocenteDinamicas: Estadísticas creadas exitosamente");
                return estadisticas;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ObtenerEstadisticasDocenteDinamicas: Error - {ex.Message}");
                return null;
            }
        }

        private async Task<UserDto?> ObtenerDatosUsuarioSession(string cedula)
        {
            try
            {
                var url = $"{_apiBaseUrl}/api/users/by-cedula/{cedula}";
                var response = await _httpClient.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var usuario = JsonSerializer.Deserialize<UserDto>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return usuario;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        private async Task<EstadisticasDocenteResponse> CrearEstadisticasDesdeVerificacionDinamica(
            string cedula, 
            VerificacionRequisitosEscalafonDto verificacion, 
            ProyectoAgiles.Application.DTOs.RequisitoEscalafonConfigDto configuracion)
        {
            try
            {
                Console.WriteLine($"CrearEstadisticasDesdeVerificacionDinamica: Iniciando");
                Console.WriteLine($"DEBUG: Verificación recibida:");
                Console.WriteLine($"  - Experiencia.Cumple: {verificacion.Experiencia.Cumple}");
                Console.WriteLine($"  - Experiencia.ValorObtenido: '{verificacion.Experiencia.ValorObtenido}'");
                Console.WriteLine($"  - ObrasRelevantes.Cumple: {verificacion.ObrasRelevantes.Cumple}");
                Console.WriteLine($"  - ObrasRelevantes.ValorObtenido: '{verificacion.ObrasRelevantes.ValorObtenido}'");
                Console.WriteLine($"  - EvaluacionDesempeno.Cumple: {verificacion.EvaluacionDesempeno.Cumple}");
                Console.WriteLine($"  - EvaluacionDesempeno.ValorObtenido: '{verificacion.EvaluacionDesempeno.ValorObtenido}'");
                Console.WriteLine($"  - Capacitacion.Cumple: {verificacion.Capacitacion.Cumple}");
                Console.WriteLine($"  - Capacitacion.ValorObtenido: '{verificacion.Capacitacion.ValorObtenido}'");
                Console.WriteLine($"  - ProyectosInvestigacion?.Cumple: {verificacion.ProyectosInvestigacion?.Cumple}");
                Console.WriteLine($"  - CumpleTodosRequisitos: {verificacion.CumpleTodosRequisitos}");
                
                // Contar requisitos totales y cumplidos usando las propiedades correctas
                var totalRequisitos = 0;
                var requisitosCumplidos = 0;

                // Verificar experiencia
                totalRequisitos++;
                if (verificacion.Experiencia.Cumple) requisitosCumplidos++;

                // Verificar obras relevantes
                totalRequisitos++;
                if (verificacion.ObrasRelevantes.Cumple) requisitosCumplidos++;

                // Verificar evaluaciones
                totalRequisitos++;
                if (verificacion.EvaluacionDesempeno.Cumple) requisitosCumplidos++;

                // Verificar capacitación
                totalRequisitos++;
                if (verificacion.Capacitacion.Cumple) requisitosCumplidos++;

                // Verificar proyectos de investigación si es requerido
                if (configuracion.RequiereProyectosInvestigacion)
                {
                    totalRequisitos++;
                    if (verificacion.ProyectosInvestigacion?.Cumple == true) requisitosCumplidos++;
                }

                var porcentaje = totalRequisitos > 0 ? (double)((decimal)requisitosCumplidos / totalRequisitos * 100) : 0;
                var puedeSubirNivel = verificacion.CumpleTodosRequisitos;

                Console.WriteLine($"CrearEstadisticasDesdeVerificacionDinamica: {requisitosCumplidos}/{totalRequisitos} requisitos ({porcentaje:F1}%)");

                // Obtener datos reales SOLO de documentos disponibles (no utilizados previamente)
                var investigaciones = await GetInvestigacionesDisponiblesPorCedula(cedula);
                var evaluaciones = await GetEvaluacionesDisponiblesPorCedula(cedula);
                var capacitaciones = await GetCapacitacionesDisponiblesPorCedula(cedula);

                // Parsear valores con logs detallados
                var añosExperiencia = ParsearAños(verificacion.Experiencia.ValorObtenido);
                var porcentajeEvaluaciones = ParsearPorcentaje(verificacion.EvaluacionDesempeno.ValorObtenido);
                var horasCapacitacion = ParsearHoras(verificacion.Capacitacion.ValorObtenido);
                var horasPedagogicas = ParsearHorasPedagogicas(verificacion.Capacitacion.ValorObtenido);
                var obrasConUTA = ParsearObrasUTA(verificacion.ObrasRelevantes.ValorObtenido);

                Console.WriteLine($"DEBUG: Valores parseados:");
                Console.WriteLine($"  - añosExperiencia: {añosExperiencia} (de '{verificacion.Experiencia.ValorObtenido}')");
                Console.WriteLine($"  - porcentajeEvaluaciones: {porcentajeEvaluaciones} (de '{verificacion.EvaluacionDesempeno.ValorObtenido}')");
                Console.WriteLine($"  - horasCapacitacion: {horasCapacitacion} (de '{verificacion.Capacitacion.ValorObtenido}')");
                Console.WriteLine($"  - horasPedagogicas: {horasPedagogicas} (de '{verificacion.Capacitacion.ValorObtenido}')");
                Console.WriteLine($"  - obrasConUTA: {obrasConUTA} (de '{verificacion.ObrasRelevantes.ValorObtenido}')");

                return new EstadisticasDocenteResponse
                {
                    Cedula = cedula,
                    Resumen = new ResumenEstadisticas
                    {
                        TotalRequisitos = totalRequisitos,
                        RequisitosCumplidos = requisitosCumplidos,
                        PorcentajeCompletitud = porcentaje,
                        PuedeSubirNivel = puedeSubirNivel
                    },
                    Secciones = new SeccionesEstadisticas
                    {
                        Experiencia = new SeccionEstadistica
                        {
                            Titulo = "Experiencia Docente",
                            Icono = "fas fa-clock",
                            Datos = new DatosSeccion
                            {
                                AñosRequeridos = configuracion.AnosExperienciaRequeridos,
                                AñosObtenidos = (double)añosExperiencia,
                                Cumple = verificacion.Experiencia.Cumple,
                                Detalles = verificacion.Experiencia.Mensaje
                            }
                        },
                        Obras = new SeccionEstadistica
                        {
                            Titulo = "Obras Relevantes",
                            Icono = "fas fa-book",
                            Datos = new DatosSeccion
                            {
                                TotalObras = investigaciones?.Count ?? 0,
                                ObrasConUTA = obrasConUTA,
                                Cumple = verificacion.ObrasRelevantes.Cumple,
                                Detalles = GenerarDetallesObrasUTA(investigaciones?.Count ?? 0, obrasConUTA, configuracion, verificacion.ObrasRelevantes.Cumple)
                            }
                        },
                        Evaluaciones = new SeccionEstadistica
                        {
                            Titulo = "Evaluaciones de Desempeño",
                            Icono = "fas fa-star",
                            Datos = new DatosSeccion
                            {
                                EvaluacionesAnalizadas = evaluaciones?.Count ?? 0,
                                PromedioObtenido = porcentajeEvaluaciones,
                                Requiere75 = configuracion.PorcentajeEvaluacionMinimo,
                                Cumple = verificacion.EvaluacionDesempeno.Cumple,
                                Detalles = verificacion.EvaluacionDesempeno.Mensaje
                            }
                        },
                        Capacitaciones = new SeccionEstadistica
                        {
                            Titulo = "Capacitaciones DITIC",
                            Icono = "fas fa-graduation-cap",
                            Datos = new DatosSeccion
                            {
                                HorasRequeridas = configuracion.HorasCapacitacionRequeridas,
                                HorasPedagogicasRequeridas = configuracion.HorasCapacitacionPedagogicas,
                                HorasObtenidas = horasCapacitacion,
                                HorasPedagogicasObtenidas = horasPedagogicas,
                                Cumple = verificacion.Capacitacion.Cumple,
                                Detalles = verificacion.Capacitacion.Mensaje
                            }
                        },
                        Proyectos = new SeccionEstadistica
                        {
                            Titulo = "Proyectos de Investigación",
                            Icono = "fas fa-flask",
                            Datos = new DatosSeccion
                            {
                                MesesRequeridos = configuracion.MesesProyectosInvestigacion,
                                MesesObtenidos = ParsearMesesInvestigacion(verificacion.ProyectosInvestigacion?.ValorObtenido),
                                ProyectosActivos = 0, // TODO: Implementar consulta
                                Cumple = verificacion.ProyectosInvestigacion?.Cumple ?? !configuracion.RequiereProyectosInvestigacion,
                                Detalles = verificacion.ProyectosInvestigacion?.Mensaje ?? (configuracion.RequiereProyectosInvestigacion ? "No se requieren proyectos para este nivel" : "Sin información de proyectos")
                            }
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CrearEstadisticasDesdeVerificacionDinamica: Error - {ex.Message}");
                return CreateDefaultEstadisticas(cedula);
            }
        }

        private decimal ParsearAños(string valorObtenido)
        {
            try
            {
                Console.WriteLine($"ParsearAños: entrada = '{valorObtenido}'");
                
                // Buscar patrón de años (ej: "4.5 años", "4,5 años de experiencia")
                var match = System.Text.RegularExpressions.Regex.Match(
                    valorObtenido, 
                    @"(\d+(?:[\.,]\d+)?)\s*años?", 
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    
                if (match.Success)
                {
                    var valorStr = match.Groups[1].Value.Replace(',', '.');
                    if (decimal.TryParse(valorStr, System.Globalization.NumberStyles.Float, 
                        System.Globalization.CultureInfo.InvariantCulture, out var años))
                    {
                        Console.WriteLine($"ParsearAños: resultado = {años}");
                        return años;
                    }
                }
                
                Console.WriteLine($"ParsearAños: no se pudo parsear, devuelve 0");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ParsearAños: error = {ex.Message}");
                return 0;
            }
        }

        private decimal ParsearPorcentaje(string valorObtenido)
        {
            try
            {
                Console.WriteLine($"ParsearPorcentaje: entrada = '{valorObtenido}'");
                
                // Buscar patrón de porcentaje (ej: "85.0%", "85%", "85.0% promedio")
                var match = System.Text.RegularExpressions.Regex.Match(
                    valorObtenido, 
                    @"(\d+(?:[\.,]\d+)?)\s*%", 
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    
                if (match.Success)
                {
                    var valorStr = match.Groups[1].Value.Replace(',', '.');
                    if (decimal.TryParse(valorStr, System.Globalization.NumberStyles.Float, 
                        System.Globalization.CultureInfo.InvariantCulture, out var porcentaje))
                    {
                        Console.WriteLine($"ParsearPorcentaje: resultado = {porcentaje}");
                        return porcentaje;
                    }
                }
                
                Console.WriteLine($"ParsearPorcentaje: no se pudo parsear, devuelve 0");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ParsearPorcentaje: error = {ex.Message}");
                return 0;
            }
        }

        private int ParsearHoras(string valorObtenido)
        {
            try
            {
                Console.WriteLine($"ParsearHoras: entrada = '{valorObtenido}'");
                
                // Buscar patrón de horas totales (ej: "40h totales", "40h")
                var match = System.Text.RegularExpressions.Regex.Match(
                    valorObtenido, 
                    @"(\d+)\s*h(?:\s+totales)?", 
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    
                if (match.Success && int.TryParse(match.Groups[1].Value, out var horas))
                {
                    Console.WriteLine($"ParsearHoras: resultado = {horas}");
                    return horas;
                }
                
                Console.WriteLine($"ParsearHoras: no se pudo parsear, devuelve 0");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ParsearHoras: error = {ex.Message}");
                return 0;
            }
        }

        private int ParsearHorasPedagogicas(string valorObtenido)
        {
            try
            {
                Console.WriteLine($"ParsearHorasPedagogicas: entrada = '{valorObtenido}'");
                
                // Buscar patrón de horas pedagógicas (ej: "(20h pedagógicas)", "20h pedagógicas")
                var match = System.Text.RegularExpressions.Regex.Match(
                    valorObtenido, 
                    @"\(?(\d+)\s*h\s+pedagógicas?\)?", 
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    
                if (match.Success && int.TryParse(match.Groups[1].Value, out var horasPedagogicas))
                {
                    Console.WriteLine($"ParsearHorasPedagogicas: resultado = {horasPedagogicas}");
                    return horasPedagogicas;
                }
                
                Console.WriteLine($"ParsearHorasPedagogicas: no se pudo parsear, devuelve 0");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ParsearHorasPedagogicas: error = {ex.Message}");
                return 0;
            }
        }

        private int ParsearObrasUTA(string valorObtenido)
        {
            try
            {
                Console.WriteLine($"ParsearObrasUTA: entrada = '{valorObtenido}'");
                
                // Patrón 1: "X obra(s) total, Y con filiación UTA" (extraer Y)
                var matchPattern1 = System.Text.RegularExpressions.Regex.Match(
                    valorObtenido, 
                    @"(\d+)\s+obra\(s\)\s+total,\s+(\d+)\s+con\s+filiación\s+UTA", 
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    
                if (matchPattern1.Success && int.TryParse(matchPattern1.Groups[2].Value, out var obrasUTA1))
                {
                    Console.WriteLine($"ParsearObrasUTA: Pattern 1 - resultado = {obrasUTA1}");
                    return obrasUTA1;
                }
                
                // Patrón 2: "Insuficientes obras con UTA: X/Y" (extraer X)
                var matchPattern2 = System.Text.RegularExpressions.Regex.Match(
                    valorObtenido, 
                    @"obras\s+con\s+UTA:\s+(\d+)/(\d+)", 
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    
                if (matchPattern2.Success && int.TryParse(matchPattern2.Groups[1].Value, out var obrasUTA2))
                {
                    Console.WriteLine($"ParsearObrasUTA: Pattern 2 - resultado = {obrasUTA2}");
                    return obrasUTA2;
                }
                
                // Patrón 3: "X con filiación UTA" (extraer X)
                var matchPattern3 = System.Text.RegularExpressions.Regex.Match(
                    valorObtenido, 
                    @"(\d+)\s+con\s+filiación\s+UTA", 
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    
                if (matchPattern3.Success && int.TryParse(matchPattern3.Groups[1].Value, out var obrasUTA3))
                {
                    Console.WriteLine($"ParsearObrasUTA: Pattern 3 - resultado = {obrasUTA3}");
                    return obrasUTA3;
                }
                
                Console.WriteLine($"ParsearObrasUTA: no se pudo parsear con ningún patrón, devuelve 0");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ParsearObrasUTA: error = {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Extrae el número de meses desde una cadena tipo "12 meses estimados desde la investigación UTA más antigua (fecha)".
        /// Si no se puede extraer, devuelve 0.
        /// </summary>
        private int ParsearMesesInvestigacion(string? valorObtenido)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(valorObtenido)) return 0;
                
                // Buscar patrón de meses: "12 meses" al inicio
                var match = System.Text.RegularExpressions.Regex.Match(
                    valorObtenido,
                    @"^(\d+)\s+meses",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    
                if (match.Success && int.TryParse(match.Groups[1].Value, out var meses))
                {
                    Console.WriteLine($"ParsearMesesInvestigacion: resultado = {meses}");
                    return meses;
                }
                
                Console.WriteLine($"ParsearMesesInvestigacion: no se pudo parsear, devuelve 0");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ParsearMesesInvestigacion: error = {ex.Message}");
                return 0;
            }
        }

        private EstadisticasDocenteResponse CreateDefaultEstadisticas(string cedula)
        {
            return new EstadisticasDocenteResponse
            {
                Cedula = cedula,
                Resumen = new ResumenEstadisticas 
                { 
                    TotalRequisitos = 5, // Actualizado a 5 requisitos incluyendo Proyectos
                    RequisitosCumplidos = 0, 
                    PorcentajeCompletitud = 0, 
                    PuedeSubirNivel = false 
                },
                Secciones = new SeccionesEstadisticas
                {
                    Experiencia = new SeccionEstadistica 
                    { 
                        Titulo = "Experiencia Docente", 
                        Icono = "fas fa-chalkboard-teacher",
                        Datos = new DatosSeccion { Cumple = false, Detalles = "Sin información disponible" }
                    },
                    Obras = new SeccionEstadistica 
                    { 
                        Titulo = "Obras/Publicaciones", 
                        Icono = "fas fa-book",
                        Datos = new DatosSeccion { Cumple = false, Detalles = "Sin información disponible" }
                    },
                    Evaluaciones = new SeccionEstadistica 
                    { 
                        Titulo = "Evaluaciones DAC", 
                        Icono = "fas fa-star",
                        Datos = new DatosSeccion { Cumple = false, Detalles = "Sin información disponible" }
                    },
                    Capacitaciones = new SeccionEstadistica 
                    { 
                        Titulo = "Capacitaciones DITIC", 
                        Icono = "fas fa-graduation-cap",
                        Datos = new DatosSeccion { Cumple = false, Detalles = "Sin información disponible" }
                    },
                    Proyectos = new SeccionEstadistica 
                    { 
                        Titulo = "Proyectos de Investigación", 
                        Icono = "fas fa-flask",
                        Datos = new DatosSeccion { Cumple = false, Detalles = "Sin información disponible" }
                    }
                }
            };
        }

        private string GenerarDetallesObrasUTA(int totalObras, int obrasConUTA, RequisitoEscalafonConfigDto configuracion, bool cumple)
        {
            var obrasUTARequeridas = configuracion.ObrasRelevantesConUTA;
            var totalObrasRequeridas = configuracion.ObrasRelevantesMinimoTotal;
            
            Console.WriteLine($"GenerarDetallesObrasUTA: totalObras={totalObras}, obrasConUTA={obrasConUTA}, requeridas UTA={obrasUTARequeridas}, total requeridas={totalObrasRequeridas}, cumple={cumple}");
            
            if (cumple)
            {
                return $"✅ Cumple: {obrasConUTA}/{obrasUTARequeridas} obras con UTA, {totalObras}/{totalObrasRequeridas} total";
            }
            else
            {
                var faltanUTA = Math.Max(0, obrasUTARequeridas - obrasConUTA);
                var faltanTotal = Math.Max(0, totalObrasRequeridas - totalObras);
                
                List<string> faltantes = new List<string>();
                
                if (faltanUTA > 0)
                {
                    faltantes.Add($"{faltanUTA} obra(s) más con filiación UTA");
                }
                
                if (faltanTotal > 0)
                {
                    faltantes.Add($"{faltanTotal} obra(s) más en total");
                }
                
                string mensaje = $"❌ Te faltan: {string.Join(" y ", faltantes)}";
                
                return mensaje;
            }
        }

        public async Task<InvestigacionDto> CrearInvestigacion(CreateInvestigacionDto createDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/api/investigaciones", createDto);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<InvestigacionDto>();
                    return result!;
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al crear investigación: {errorContent}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear investigación: {ex.Message}");
            }
        }

        public async Task<InvestigacionDto> CrearInvestigacionConPdf(CreateInvestigacionWithPdfDto createDto)
        {
            try
            {
                Console.WriteLine($"CrearInvestigacionConPdf - PDF: {createDto.ArchivoPdf?.Name}, Size: {createDto.ArchivoPdf?.Size ?? 0}");
                
                using var form = new MultipartFormDataContent();
                form.Add(new StringContent(createDto.Cedula), "Cedula");
                form.Add(new StringContent(createDto.Titulo), "Titulo");
                form.Add(new StringContent(createDto.Tipo), "Tipo");
                form.Add(new StringContent(createDto.RevistaOEditorial), "RevistaOEditorial");
                form.Add(new StringContent(createDto.FechaPublicacion.ToString("o")), "FechaPublicacion");
                form.Add(new StringContent(createDto.CampoConocimiento), "CampoConocimiento");
                form.Add(new StringContent(createDto.Filiacion), "Filiacion");
                form.Add(new StringContent(createDto.Observacion), "Observacion");
                
                if (createDto.ArchivoPdf != null)
                {
                    var stream = createDto.ArchivoPdf.OpenReadStream(10 * 1024 * 1024); // 10MB máx
                    form.Add(new StreamContent(stream), "ArchivoPdf", createDto.ArchivoPdf.Name);
                    Console.WriteLine($"CrearInvestigacionConPdf - PDF agregado al form: {createDto.ArchivoPdf.Name}");
                }
                else
                {
                    Console.WriteLine("CrearInvestigacionConPdf - No hay PDF para enviar");
                }
                
                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/investigaciones/con-pdf", form);
                Console.WriteLine($"CrearInvestigacionConPdf - Response: {response.StatusCode}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<InvestigacionDto>() ?? new InvestigacionDto();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CrearInvestigacionConPdf - Error: {ex.Message}");
                throw;
            }
        }        public async Task<InvestigacionDto> ActualizarInvestigacion(UpdateInvestigacionDto updateDto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/api/investigaciones/{updateDto.Id}", updateDto);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<InvestigacionDto>();
                    return result!;
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al actualizar investigación: {errorContent}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar investigación: {ex.Message}");
            }
        }

        public async Task<InvestigacionDto> ActualizarInvestigacionConPdf(UpdateInvestigacionWithPdfDto updateDto)
        {
            try
            {
                Console.WriteLine($"ActualizarInvestigacionConPdf - ID: {updateDto.Id}, PDF: {updateDto.ArchivoPdf?.Name}, Size: {updateDto.ArchivoPdf?.Size ?? 0}");
                
                using var form = new MultipartFormDataContent();
                form.Add(new StringContent(updateDto.Id.ToString()), "Id");
                form.Add(new StringContent(updateDto.Cedula), "Cedula");
                form.Add(new StringContent(updateDto.Titulo), "Titulo");
                form.Add(new StringContent(updateDto.Tipo), "Tipo");
                form.Add(new StringContent(updateDto.RevistaOEditorial), "RevistaOEditorial");
                form.Add(new StringContent(updateDto.FechaPublicacion.ToString("o")), "FechaPublicacion");
                form.Add(new StringContent(updateDto.CampoConocimiento), "CampoConocimiento");
                form.Add(new StringContent(updateDto.Filiacion), "Filiacion");
                form.Add(new StringContent(updateDto.Observacion), "Observacion");
                
                if (updateDto.ArchivoPdf != null)
                {
                    var stream = updateDto.ArchivoPdf.OpenReadStream(10 * 1024 * 1024); // 10MB máx
                    form.Add(new StreamContent(stream), "ArchivoPdf", updateDto.ArchivoPdf.Name);
                    Console.WriteLine($"ActualizarInvestigacionConPdf - PDF agregado al form: {updateDto.ArchivoPdf.Name}");
                }
                else
                {
                    Console.WriteLine("ActualizarInvestigacionConPdf - No hay PDF para actualizar");
                }

                var response = await _httpClient.PutAsync($"{_apiBaseUrl}/api/investigaciones/{updateDto.Id}/con-pdf", form);
                Console.WriteLine($"ActualizarInvestigacionConPdf - Response: {response.StatusCode}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<InvestigacionDto>() ?? new InvestigacionDto();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ActualizarInvestigacionConPdf - Error: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> EliminarInvestigacion(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/api/investigaciones/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar investigación: {ex.Message}");
            }
        }        public async Task<byte[]?> ObtenerPdfInvestigacion(int investigacionId)
        {
            try
            {
                Console.WriteLine($"ObtenerPdfInvestigacion - Solicitando PDF ID: {investigacionId}");
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/investigaciones/{investigacionId}/pdf");
                Console.WriteLine($"ObtenerPdfInvestigacion - Response Status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var bytes = await response.Content.ReadAsByteArrayAsync();
                    Console.WriteLine($"ObtenerPdfInvestigacion - Bytes recibidos: {bytes.Length}");
                    return bytes;
                }
                Console.WriteLine($"ObtenerPdfInvestigacion - Error: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ObtenerPdfInvestigacion - Exception: {ex.Message}");                throw;
            }
        }

        // Métodos para trabajar con evaluaciones de desempeño
        
        public async Task<List<EvaluacionDesempenoDto>> GetEvaluacionesPorCedula(string cedula)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<EvaluacionDesempenoDto>>($"{_apiBaseUrl}/api/EvaluacionesDesempeno/by-cedula/{cedula}");
                return response ?? new List<EvaluacionDesempenoDto>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener evaluaciones: {ex.Message}");
            }
        }

        public async Task<List<EvaluacionDesempenoDto>> GetEvaluacionesDisponiblesPorCedula(string cedula)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<EvaluacionDesempenoDto>>($"{_apiBaseUrl}/api/EvaluacionesDesempeno/disponibles/{cedula}");
                return response ?? new List<EvaluacionDesempenoDto>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener evaluaciones disponibles: {ex.Message}");
            }
        }

        public async Task<EvaluacionDesempenoDto> CrearEvaluacion(CreateEvaluacionDesempenoDto createDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/api/EvaluacionesDesempeno", createDto);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<EvaluacionDesempenoDto>();
                    return result!;
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al crear evaluación: {errorContent}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear evaluación: {ex.Message}");
            }
        }

        public async Task<EvaluacionDesempenoDto> CrearEvaluacionConPdf(CreateEvaluacionDesempenoWithPdfDto createDto)
        {
            try
            {
                Console.WriteLine($"CrearEvaluacionConPdf - PDF: {createDto.ArchivoPdf?.Name}, Size: {createDto.ArchivoPdf?.Size ?? 0}");
                
                using var form = new MultipartFormDataContent();
                form.Add(new StringContent(createDto.Cedula), "Cedula");
                form.Add(new StringContent(createDto.PeriodoAcademico), "PeriodoAcademico");
                form.Add(new StringContent(createDto.Anio.ToString()), "Anio");
                form.Add(new StringContent(createDto.Semestre.ToString()), "Semestre");
                form.Add(new StringContent(createDto.PuntajeObtenido.ToString()), "PuntajeObtenido");
                form.Add(new StringContent(createDto.PuntajeMaximo.ToString()), "PuntajeMaximo");
                form.Add(new StringContent(createDto.FechaEvaluacion.ToString("o")), "FechaEvaluacion");
                form.Add(new StringContent(createDto.TipoEvaluacion), "TipoEvaluacion");
                form.Add(new StringContent(createDto.Estado), "Estado");
                
                if (!string.IsNullOrEmpty(createDto.Observaciones))
                    form.Add(new StringContent(createDto.Observaciones), "Observaciones");
                
                if (!string.IsNullOrEmpty(createDto.Evaluador))
                    form.Add(new StringContent(createDto.Evaluador), "Evaluador");
                
                if (createDto.ArchivoPdf != null)
                {
                    var stream = createDto.ArchivoPdf.OpenReadStream(10 * 1024 * 1024); // 10MB máx
                    form.Add(new StreamContent(stream), "ArchivoPdf", createDto.ArchivoPdf.Name);
                    Console.WriteLine($"CrearEvaluacionConPdf - PDF agregado al form: {createDto.ArchivoPdf.Name}");
                }
                else
                {
                    Console.WriteLine("CrearEvaluacionConPdf - No hay PDF para enviar");
                }
                
                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/EvaluacionesDesempeno/con-pdf", form);
                Console.WriteLine($"CrearEvaluacionConPdf - Response: {response.StatusCode}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<EvaluacionDesempenoDto>() ?? new EvaluacionDesempenoDto();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CrearEvaluacionConPdf - Error: {ex.Message}");
                throw;
            }
        }

        public async Task<EvaluacionDesempenoDto> ActualizarEvaluacion(UpdateEvaluacionDesempenoDto updateDto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/api/EvaluacionesDesempeno/{updateDto.Id}", updateDto);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<EvaluacionDesempenoDto>();
                    return result!;
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al actualizar evaluación: {errorContent}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar evaluación: {ex.Message}");
            }
        }

        public async Task<EvaluacionDesempenoDto> ActualizarEvaluacionConPdf(UpdateEvaluacionDesempenoWithPdfDto updateDto)
        {
            try
            {
                Console.WriteLine($"ActualizarEvaluacionConPdf - ID: {updateDto.Id}, PDF: {updateDto.ArchivoPdf?.Name}, Size: {updateDto.ArchivoPdf?.Size ?? 0}");
                
                using var form = new MultipartFormDataContent();
                form.Add(new StringContent(updateDto.Id.ToString()), "Id");
                form.Add(new StringContent(updateDto.Cedula), "Cedula");
                form.Add(new StringContent(updateDto.PeriodoAcademico), "PeriodoAcademico");
                form.Add(new StringContent(updateDto.Anio.ToString()), "Anio");
                form.Add(new StringContent(updateDto.Semestre.ToString()), "Semestre");
                form.Add(new StringContent(updateDto.PuntajeObtenido.ToString()), "PuntajeObtenido");
                form.Add(new StringContent(updateDto.PuntajeMaximo.ToString()), "PuntajeMaximo");
                form.Add(new StringContent(updateDto.FechaEvaluacion.ToString("o")), "FechaEvaluacion");
                form.Add(new StringContent(updateDto.TipoEvaluacion), "TipoEvaluacion");
                form.Add(new StringContent(updateDto.Estado), "Estado");
                
                if (!string.IsNullOrEmpty(updateDto.Observaciones))
                    form.Add(new StringContent(updateDto.Observaciones), "Observaciones");
                
                if (!string.IsNullOrEmpty(updateDto.Evaluador))
                    form.Add(new StringContent(updateDto.Evaluador), "Evaluador");
                
                if (updateDto.ArchivoPdf != null)
                {
                    var stream = updateDto.ArchivoPdf.OpenReadStream(10 * 1024 * 1024); // 10MB máx
                    form.Add(new StreamContent(stream), "ArchivoPdf", updateDto.ArchivoPdf.Name);
                    Console.WriteLine($"ActualizarEvaluacionConPdf - PDF agregado al form: {updateDto.ArchivoPdf.Name}");
                }
                else
                {
                    Console.WriteLine("ActualizarEvaluacionConPdf - No hay PDF para actualizar");
                }

                var response = await _httpClient.PutAsync($"{_apiBaseUrl}/api/EvaluacionesDesempeno/{updateDto.Id}/con-pdf", form);
                Console.WriteLine($"ActualizarEvaluacionConPdf - Response: {response.StatusCode}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<EvaluacionDesempenoDto>() ?? new EvaluacionDesempenoDto();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ActualizarEvaluacionConPdf - Error: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> EliminarEvaluacion(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/api/EvaluacionesDesempeno/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar evaluación: {ex.Message}");
            }
        }

        public async Task<byte[]?> ObtenerPdfEvaluacion(int evaluacionId)
        {
            try
            {
                Console.WriteLine($"ObtenerPdfEvaluacion - Solicitando PDF ID: {evaluacionId}");
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/EvaluacionesDesempeno/{evaluacionId}/pdf");
                Console.WriteLine($"ObtenerPdfEvaluacion - Response Status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var bytes = await response.Content.ReadAsByteArrayAsync();
                    Console.WriteLine($"ObtenerPdfEvaluacion - Bytes recibidos: {bytes.Length}");
                    return bytes;
                }
                Console.WriteLine($"ObtenerPdfEvaluacion - Error: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ObtenerPdfEvaluacion - Exception: {ex.Message}");
                throw;
            }
        }

        // Métodos para trabajar con solicitudes
        
        public async Task<List<ProyectoAgiles.Application.DTOs.SolicitudEscalafonDto>> GetSolicitudesPorCedula(string cedula)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<ProyectoAgiles.Application.DTOs.SolicitudEscalafonDto>>($"{_apiBaseUrl}/api/solicitudes-escalafon/docente/{cedula}");
                return response ?? new List<ProyectoAgiles.Application.DTOs.SolicitudEscalafonDto>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener solicitudes: {ex.Message}");
            }
        }

        // Métodos para trabajar con capacitaciones DITIC
          public async Task<List<ProyectoAgiles.Application.DTOs.DiticDto>> GetCapacitacionesPorCedula(string cedula)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<ProyectoAgiles.Application.DTOs.DiticDto>>($"{_apiBaseUrl}/api/ditic/docente/{cedula}");
                return response ?? new List<ProyectoAgiles.Application.DTOs.DiticDto>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener capacitaciones: {ex.Message}");
            }
        }

        public async Task<List<ProyectoAgiles.Application.DTOs.DiticDto>> GetCapacitacionesDisponiblesPorCedula(string cedula)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<ProyectoAgiles.Application.DTOs.DiticDto>>($"{_apiBaseUrl}/api/ditic/disponibles/{cedula}");
                return response ?? new List<ProyectoAgiles.Application.DTOs.DiticDto>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener capacitaciones disponibles: {ex.Message}");
            }
        }        public async Task<ProyectoAgiles.Application.DTOs.DiticDto> CrearCapacitacion(ProyectoAgiles.Application.DTOs.CreateDiticDto createDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/api/ditic", createDto);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ProyectoAgiles.Application.DTOs.DiticDto>();
                    return result!;
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al crear capacitación: {errorContent}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear capacitación: {ex.Message}");
            }
        }public async Task<ProyectoAgiles.Application.DTOs.DiticDto> CrearCapacitacionConPdf(
            string cedula,
            string nombreCapacitacion,
            string institucion,
            string tipoCapacitacion,
            string modalidad,
            int horasAcademicas,
            DateTime fechaInicio,
            DateTime fechaFin,
            int anio,
            string estado,
            decimal? calificacion,
            decimal calificacionMinima,
            string? descripcion,
            string? numeroCertificado,
            string? instructor,
            string? observaciones,
            bool exencionPorAutoridad,
            string? cargoAutoridad,
            DateTime? fechaInicioAutoridad,
            DateTime? fechaFinAutoridad,
            IBrowserFile? archivoCertificado)
        {
            try
            {
                Console.WriteLine($"CrearCapacitacionConPdf - PDF: {archivoCertificado?.Name}, Size: {archivoCertificado?.Size ?? 0}");
                
                using var form = new MultipartFormDataContent();
                form.Add(new StringContent(cedula), "Cedula");
                form.Add(new StringContent(nombreCapacitacion), "NombreCapacitacion");
                form.Add(new StringContent(institucion), "Institucion");
                form.Add(new StringContent(tipoCapacitacion), "TipoCapacitacion");
                form.Add(new StringContent(modalidad), "Modalidad");
                form.Add(new StringContent(horasAcademicas.ToString()), "HorasAcademicas");
                form.Add(new StringContent(fechaInicio.ToString("o")), "FechaInicio");
                form.Add(new StringContent(fechaFin.ToString("o")), "FechaFin");
                form.Add(new StringContent(anio.ToString()), "Anio");
                form.Add(new StringContent(estado), "Estado");
                form.Add(new StringContent(calificacionMinima.ToString()), "CalificacionMinima");
                
                if (calificacion.HasValue)
                    form.Add(new StringContent(calificacion.Value.ToString()), "Calificacion");
                
                if (!string.IsNullOrEmpty(descripcion))
                    form.Add(new StringContent(descripcion), "Descripcion");
                
                if (!string.IsNullOrEmpty(numeroCertificado))
                    form.Add(new StringContent(numeroCertificado), "NumeroCertificado");
                
                if (!string.IsNullOrEmpty(instructor))
                    form.Add(new StringContent(instructor), "Instructor");
                
                if (!string.IsNullOrEmpty(observaciones))
                    form.Add(new StringContent(observaciones), "Observaciones");
                
                form.Add(new StringContent(exencionPorAutoridad.ToString()), "ExencionPorAutoridad");
                
                if (!string.IsNullOrEmpty(cargoAutoridad))
                    form.Add(new StringContent(cargoAutoridad), "CargoAutoridad");
                
                if (fechaInicioAutoridad.HasValue)
                    form.Add(new StringContent(fechaInicioAutoridad.Value.ToString("o")), "FechaInicioAutoridad");
                
                if (fechaFinAutoridad.HasValue)
                    form.Add(new StringContent(fechaFinAutoridad.Value.ToString("o")), "FechaFinAutoridad");
                  if (archivoCertificado != null)
                {
                    var stream = archivoCertificado.OpenReadStream(10 * 1024 * 1024); // 10MB máx
                    var pdfContent = new StreamContent(stream);
                    pdfContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                    form.Add(pdfContent, "ArchivoCertificado", archivoCertificado.Name);
                    Console.WriteLine($"CrearCapacitacionConPdf - PDF agregado al form: {archivoCertificado.Name}");
                }
                else
                {
                    Console.WriteLine("CrearCapacitacionConPdf - No hay PDF para enviar");
                }
                
                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/ditic/con-certificado", form);
                Console.WriteLine($"CrearCapacitacionConPdf - Response: {response.StatusCode}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ProyectoAgiles.Application.DTOs.DiticDto>() ?? new ProyectoAgiles.Application.DTOs.DiticDto();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CrearCapacitacionConPdf - Error: {ex.Message}");
                throw;
            }
        }        public async Task<ProyectoAgiles.Application.DTOs.DiticDto> ActualizarCapacitacion(ProyectoAgiles.Application.DTOs.UpdateDiticDto updateDto)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/api/ditic/{updateDto.Id}", updateDto);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ProyectoAgiles.Application.DTOs.DiticDto>();
                    return result!;
                }
                
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al actualizar capacitación: {errorContent}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar capacitación: {ex.Message}");
            }
        }

        public async Task<bool> EliminarCapacitacion(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}/api/ditic/{id}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar capacitación: {ex.Message}");
            }
        }

        public async Task<byte[]?> ObtenerPdfCapacitacion(int capacitacionId)
        {
            try
            {
                Console.WriteLine($"ObtenerPdfCapacitacion - Solicitando PDF ID: {capacitacionId}");
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/ditic/{capacitacionId}/certificado");
                Console.WriteLine($"ObtenerPdfCapacitacion - Response Status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var bytes = await response.Content.ReadAsByteArrayAsync();
                    Console.WriteLine($"ObtenerPdfCapacitacion - Bytes recibidos: {bytes.Length}");
                    return bytes;
                }
                Console.WriteLine($"ObtenerPdfCapacitacion - Error: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ObtenerPdfCapacitacion - Exception: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> ActualizarCertificadoCapacitacion(int id, IBrowserFile archivoCertificado)        {
            try
            {
                Console.WriteLine($"ActualizarCertificadoCapacitacion - ID: {id}, PDF: {archivoCertificado?.Name}, Size: {archivoCertificado?.Size ?? 0}");
                
                using var form = new MultipartFormDataContent();                if (archivoCertificado != null)
                {
                    var stream = archivoCertificado.OpenReadStream(10 * 1024 * 1024); // 10MB máx
                    var pdfContent = new StreamContent(stream);
                    pdfContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                    form.Add(pdfContent, "archivo", archivoCertificado.Name);
                    Console.WriteLine($"ActualizarCertificadoCapacitacion - PDF agregado al form: {archivoCertificado.Name}");
                }
                else
                {
                    throw new Exception("No se seleccionó un archivo PDF para actualizar");
                }
                
                Console.WriteLine($"ActualizarCertificadoCapacitacion - Enviando PUT a: {_apiBaseUrl}/api/ditic/{id}/certificado");
                var response = await _httpClient.PutAsync($"{_apiBaseUrl}/api/ditic/{id}/certificado", form);
                Console.WriteLine($"ActualizarCertificadoCapacitacion - Response: {response.StatusCode}");
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"ActualizarCertificadoCapacitacion - Error content: {errorContent}");
                    throw new Exception($"Error del servidor: {response.StatusCode} - {errorContent}");
                }
                
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ActualizarCertificadoCapacitacion - Exception: {ex.Message}");
                throw new Exception($"Error al actualizar el certificado PDF: {ex.Message}");
            }
        }

        /// <summary>
        /// Verifica los requisitos de escalafón dinámicamente según el nivel actual del docente
        /// </summary>
        /// <param name="cedula">Cédula del docente</param>
        /// <param name="nivelActual">Nivel académico actual del docente</param>
        /// <returns>Verificación completa de requisitos para el siguiente nivel</returns>
        public async Task<VerificacionRequisitosEscalafonDto> VerificarRequisitosEscalafonDinamico(string cedula, string nivelActual)
        {
            try
            {
                // Obtener configuración de requisitos para el nivel actual
                var requisitosService = new ProyectoAgiles.Application.Services.RequisitosEscalafonService();
                var config = requisitosService.GetRequisitosParaNivel(nivelActual);

                var verificacion = new VerificacionRequisitosEscalafonDto
                {
                    Cedula = cedula,
                    NivelActual = config.NivelActual,
                    NivelObjetivo = config.NivelObjetivo,
                    ConfiguracionRequisitos = config,
                    CumpleTodosRequisitos = false,
                    Mensaje = $"Verificando requisitos para ascender de {config.NivelActual} a {config.NivelObjetivo}..."
                };

                // 1. Verificar experiencia mínima
                verificacion.Experiencia = await VerificarExperienciaMinimaDinamica(cedula, config);

                // 2. Verificar obras relevantes
                verificacion.ObrasRelevantes = await VerificarObrasRelevantesDinamica(cedula, config);

                // 3. Verificar evaluación de desempeño
                verificacion.EvaluacionDesempeno = await VerificarEvaluacionDesempenoDinamica(cedula, config);

                // 4. Verificar capacitación
                verificacion.Capacitacion = await VerificarCapacitacionDinamica(cedula, config);

                // 5. Verificar proyectos de investigación (si aplica)
                if (config.RequiereProyectosInvestigacion)
                {
                    verificacion.ProyectosInvestigacion = await VerificarProyectosInvestigacionDinamica(cedula, config);
                    Console.WriteLine($"DEBUG: ProyectosInvestigacion requerido - {verificacion.ProyectosInvestigacion.Mensaje}");
                }
                else
                {
                    Console.WriteLine($"DEBUG: ProyectosInvestigacion NO requerido para este nivel");
                }

                // Determinar si cumple todos los requisitos
                verificacion.CumpleTodosRequisitos = 
                    verificacion.Experiencia.Cumple &&
                    verificacion.ObrasRelevantes.Cumple &&
                    verificacion.EvaluacionDesempeno.Cumple &&
                    verificacion.Capacitacion.Cumple &&
                    (verificacion.ProyectosInvestigacion?.Cumple ?? true);

                Console.WriteLine($"DEBUG VerificarRequisitosEscalafonDinamico:");
                Console.WriteLine($"  - Experiencia: {verificacion.Experiencia.Cumple} - {verificacion.Experiencia.Mensaje}");
                Console.WriteLine($"  - ObrasRelevantes: {verificacion.ObrasRelevantes.Cumple} - {verificacion.ObrasRelevantes.Mensaje}");
                Console.WriteLine($"  - EvaluacionDesempeno: {verificacion.EvaluacionDesempeno.Cumple} - {verificacion.EvaluacionDesempeno.Mensaje}");
                Console.WriteLine($"  - Capacitacion: {verificacion.Capacitacion.Cumple} - {verificacion.Capacitacion.Mensaje}");
                Console.WriteLine($"  - ProyectosInvestigacion: {verificacion.ProyectosInvestigacion?.Cumple} - {verificacion.ProyectosInvestigacion?.Mensaje}");
                Console.WriteLine($"  - CumpleTodosRequisitos: {verificacion.CumpleTodosRequisitos}");

                // Generar mensaje final
                if (verificacion.CumpleTodosRequisitos)
                {
                    verificacion.Mensaje = $"✅ CUMPLE con todos los requisitos para ascender de {config.NivelActual} a {config.NivelObjetivo}";
                }
                else
                {
                    verificacion.Mensaje = $"❌ NO CUMPLE con los siguientes requisitos: {string.Join(", ", verificacion.RequisitosIncumplidos)}";
                }

                return verificacion;
            }
            catch (Exception ex)
            {
                return new VerificacionRequisitosEscalafonDto
                {
                    Cedula = cedula,
                    NivelActual = nivelActual,
                    NivelObjetivo = "Error",
                    CumpleTodosRequisitos = false,
                    Mensaje = $"Error al verificar requisitos: {ex.Message}"
                };
            }
        }

        private async Task<RequisitoCumplimientoDto> VerificarExperienciaMinimaDinamica(string cedula, RequisitoEscalafonConfigDto config)
        {
            try
            {
                // Para verificar experiencia mínima, necesitamos la fecha de cuando obtuvo el nivel actual
                // Por ejemplo, si está en "Titular Auxiliar 2" y quiere subir a "Titular Agregado 1",
                // necesitamos verificar que hayan pasado 4 años desde que ascendió a "Titular Auxiliar 2"

                DateTime? fechaInicioNivelActual = null;
                string fuente = "";

                // 1. Primero intentar obtener la fecha de ascenso desde TTHH (si tiene información del nivel actual)
                var tthhResponse = await _httpClient.GetAsync($"{_apiBaseUrl}/api/tthh/by-cedula/{cedula}");
                if (tthhResponse.IsSuccessStatusCode)
                {
                    var tthhInfo = await tthhResponse.Content.ReadFromJsonAsync<TTHHDto>();
                    if (tthhInfo != null)
                    {
                        // Usar la fecha de inicio registrada en TTHH
                        fechaInicioNivelActual = tthhInfo.FechaInicio;
                        fuente = "TTHH (fecha de inicio registrada)";
                    }
                }

                // 2. Si no hay información de TTHH o no es confiable, buscar en historial de solicitudes de escalafón
                if (!fechaInicioNivelActual.HasValue)
                {
                    try
                    {
                        var solicitudesResponse = await _httpClient.GetAsync($"{_apiBaseUrl}/api/solicitudes-escalafon/docente/{cedula}");
                        if (solicitudesResponse.IsSuccessStatusCode)
                        {
                            var solicitudes = await solicitudesResponse.Content.ReadFromJsonAsync<List<ProyectoAgiles.Application.DTOs.SolicitudEscalafonDto>>();
                            if (solicitudes != null && solicitudes.Count > 0)
                            {
                                // Buscar la solicitud aprobada más reciente que haya resultado en el nivel actual
                                var solicitudNivelActual = solicitudes
                                    .Where(s => s.Status.Equals("Aprobada", StringComparison.OrdinalIgnoreCase) && 
                                               s.NivelSolicitado.Equals(config.NivelActual, StringComparison.OrdinalIgnoreCase))
                                    .OrderByDescending(s => s.FechaAprobacion ?? s.FechaSolicitud)
                                    .FirstOrDefault();

                                if (solicitudNivelActual != null)
                                {
                                    fechaInicioNivelActual = solicitudNivelActual.FechaAprobacion ?? solicitudNivelActual.FechaSolicitud;
                                    fuente = "Historial de solicitudes de escalafón (fecha de aprobación)";
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al consultar historial de solicitudes: {ex.Message}");
                    }
                }

                // 3. Fallback: usar fecha de registro del usuario como estimación (menos preciso)
                if (!fechaInicioNivelActual.HasValue)
                {
                    var userResponse = await _httpClient.GetAsync($"{_apiBaseUrl}/api/users/by-cedula/{cedula}");
                    if (userResponse.IsSuccessStatusCode)
                    {
                        var userInfo = await userResponse.Content.ReadFromJsonAsync<UserDto>();
                        if (userInfo != null)
                        {
                            fechaInicioNivelActual = userInfo.CreatedAt;
                            fuente = "Fecha de registro de usuario (estimación menos precisa)";
                        }
                    }
                }

                // Calcular años de experiencia en el nivel actual
                if (fechaInicioNivelActual.HasValue)
                {
                    var añosEnNivelActual = (DateTime.Now - fechaInicioNivelActual.Value).TotalDays / 365.25;
                    var cumple = añosEnNivelActual >= config.AnosExperienciaRequeridos;
                    
                    string mensaje;
                    if (cumple)
                    {
                        mensaje = $"✅ Cumple experiencia: {añosEnNivelActual:F1} años como {config.NivelActual} desde {fechaInicioNivelActual.Value:dd/MM/yyyy}";
                    }
                    else
                    {
                        mensaje = $"❌ No cumple experiencia: {añosEnNivelActual:F1} años como {config.NivelActual} desde {fechaInicioNivelActual.Value:dd/MM/yyyy} " +
                                 $"(requiere mínimo {config.AnosExperienciaRequeridos} años)";
                    }

                    return new RequisitoCumplimientoDto
                    {
                        Cumple = cumple,
                        Mensaje = mensaje,
                        ValorObtenido = $"{añosEnNivelActual:F1} años como {config.NivelActual} (desde {fechaInicioNivelActual.Value:dd/MM/yyyy})",
                        ValorRequerido = $"{config.AnosExperienciaRequeridos} años mínimo como {config.NivelActual}. Fuente: {fuente}"
                    };
                }
                
                return new RequisitoCumplimientoDto
                {
                    Cumple = false,
                    Mensaje = $"❌ No se pudo verificar la experiencia mínima como {config.NivelActual} - Sin datos de fecha de ascenso disponibles",
                    ValorObtenido = "No disponible - Falta fecha de ascenso al nivel actual",
                    ValorRequerido = $"{config.AnosExperienciaRequeridos} años mínimo como {config.NivelActual}. Se requiere registrar la fecha de ascenso al nivel actual en TTHH o tener historial de solicitudes"
                };
            }
            catch (Exception ex)
            {
                return new RequisitoCumplimientoDto
                {
                    Cumple = false,
                    Mensaje = $"❌ Error al verificar experiencia: {ex.Message}",
                    ValorObtenido = "Error",
                    ValorRequerido = $"{config.AnosExperienciaRequeridos} años mínimo como {config.NivelActual}"
                };
            }
        }

        private async Task<RequisitoCumplimientoDto> VerificarObrasRelevantesDinamica(string cedula, RequisitoEscalafonConfigDto config)
        {
            try
            {
                var investigaciones = await GetInvestigacionesDisponiblesPorCedula(cedula);
                
                // Contar obras con filiación UTA
                var investigacionesUTA = investigaciones.Where(i => 
                    i.Filiacion.Contains("UTA", StringComparison.OrdinalIgnoreCase) ||
                    i.Filiacion.Contains("Universidad Técnica de Ambato", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                var totalObras = investigaciones.Count;
                var obrasUTA = investigacionesUTA.Count;
                var cumpleTotal = totalObras >= config.ObrasRelevantesMinimoTotal;
                var cumpleUTA = obrasUTA >= config.ObrasRelevantesConUTA;
                var cumple = cumpleTotal && cumpleUTA;

                string mensaje;
                if (cumple)
                {
                    mensaje = $"✅ Cumple obras relevantes: {totalObras} total ({obrasUTA} con filiación UTA)";
                }
                else if (!cumpleTotal && !cumpleUTA)
                {
                    mensaje = $"❌ No cumple obras relevantes: {totalObras}/{config.ObrasRelevantesMinimoTotal} total, {obrasUTA}/{config.ObrasRelevantesConUTA} con UTA";
                }
                else if (!cumpleTotal)
                {
                    mensaje = $"❌ Insuficientes obras totales: {totalObras}/{config.ObrasRelevantesMinimoTotal}";
                }
                else
                {
                    mensaje = $"❌ Insuficientes obras con UTA: {obrasUTA}/{config.ObrasRelevantesConUTA}";
                }

                return new RequisitoCumplimientoDto
                {
                    Cumple = cumple,
                    Mensaje = mensaje,
                    ValorObtenido = $"{totalObras} obra(s) total, {obrasUTA} con filiación UTA",
                    ValorRequerido = $"Mínimo {config.ObrasRelevantesMinimoTotal} obra(s) total, {config.ObrasRelevantesConUTA} con filiación UTA"
                };
            }
            catch (Exception ex)
            {
                return new RequisitoCumplimientoDto
                {
                    Cumple = false,
                    Mensaje = $"❌ Error al verificar obras relevantes: {ex.Message}",
                    ValorObtenido = "Error",
                    ValorRequerido = $"Mínimo {config.ObrasRelevantesMinimoTotal} obra(s) total, {config.ObrasRelevantesConUTA} con filiación UTA"
                };
            }
        }

        private async Task<RequisitoCumplimientoDto> VerificarEvaluacionDesempenoDinamica(string cedula, RequisitoEscalafonConfigDto config)
        {
            try
            {
                // Obtener solo evaluaciones disponibles (no utilizadas)
                var evaluaciones = await GetEvaluacionesDisponiblesPorCedula(cedula);
                
                if (!evaluaciones.Any())
                {
                    return new RequisitoCumplimientoDto
                    {
                        Cumple = false,
                        Mensaje = "❌ No hay evaluaciones disponibles para verificar",
                        ValorObtenido = "0 evaluaciones",
                        ValorRequerido = $"{config.PorcentajeEvaluacionMinimo}% mínimo en últimos {config.PeriodosEvaluacionRequeridos} períodos"
                    };
                }

                // Ordenar por año y semestre (más recientes primero)
                var evaluacionesOrdenadas = evaluaciones
                    .OrderByDescending(e => e.Anio)
                    .ThenByDescending(e => e.Semestre)
                    .Take(config.PeriodosEvaluacionRequeridos)
                    .ToList();

                if (evaluacionesOrdenadas.Count < config.PeriodosEvaluacionRequeridos)
                {
                    return new RequisitoCumplimientoDto
                    {
                        Cumple = false,
                        Mensaje = $"❌ Insuficientes evaluaciones: {evaluacionesOrdenadas.Count}/{config.PeriodosEvaluacionRequeridos} períodos",
                        ValorObtenido = $"{evaluacionesOrdenadas.Count} evaluaciones",
                        ValorRequerido = $"{config.PorcentajeEvaluacionMinimo}% mínimo en últimos {config.PeriodosEvaluacionRequeridos} períodos"
                    };
                }

                // Calcular el promedio de las evaluaciones consideradas
                var promedioObtenido = evaluacionesOrdenadas.Average(e => 
                    e.PuntajeMaximo > 0 ? (e.PuntajeObtenido / e.PuntajeMaximo) * 100 : 0);

                var cumple = promedioObtenido >= config.PorcentajeEvaluacionMinimo;

                return new RequisitoCumplimientoDto
                {
                    Cumple = cumple,
                    Mensaje = cumple 
                        ? $"✅ Cumple evaluación: {promedioObtenido:F1}% promedio en {evaluacionesOrdenadas.Count} períodos"
                        : $"❌ No cumple evaluación: {promedioObtenido:F1}% promedio en {evaluacionesOrdenadas.Count} períodos",
                    ValorObtenido = $"{promedioObtenido:F1}% promedio",
                    ValorRequerido = $"{config.PorcentajeEvaluacionMinimo}% mínimo en últimos {config.PeriodosEvaluacionRequeridos} períodos"
                };
            }
            catch (Exception ex)
            {
                return new RequisitoCumplimientoDto
                {
                    Cumple = false,
                    Mensaje = $"❌ Error al verificar evaluación: {ex.Message}",
                    ValorObtenido = "Error",
                    ValorRequerido = $"{config.PorcentajeEvaluacionMinimo}% mínimo en últimos {config.PeriodosEvaluacionRequeridos} períodos"
                };
            }
        }

        private async Task<RequisitoCumplimientoDto> VerificarCapacitacionDinamica(string cedula, RequisitoEscalafonConfigDto config)
        {
            try
            {
                // Obtener solo capacitaciones disponibles (no utilizadas)
                var capacitaciones = await GetCapacitacionesDisponiblesPorCedula(cedula);
                
                if (!capacitaciones.Any())
                {
                    return new RequisitoCumplimientoDto
                    {
                        Cumple = false,
                        Mensaje = "❌ No hay capacitaciones disponibles para verificar",
                        ValorObtenido = "0 horas",
                        ValorRequerido = $"{config.HorasCapacitacionRequeridas}h totales ({config.HorasCapacitacionPedagogicas}h pedagógicas)"
                    };
                }

                // Filtrar capacitaciones de los últimos 3 años
                var fechaLimite = DateTime.Now.AddYears(-3);
                var capacitacionesRecientes = capacitaciones
                    .Where(c => c.FechaInicio >= fechaLimite)
                    .ToList();

                // Calcular horas totales y pedagógicas
                var horasTotales = capacitacionesRecientes.Sum(c => c.HorasAcademicas);
                
                // Log detallado para debugging
                Console.WriteLine($"DEBUG VerificarCapacitacionDinamica - Calculando horas pedagógicas:");
                foreach (var cap in capacitacionesRecientes)
                {
                    Console.WriteLine($"  - '{cap.NombreCapacitacion}' | Tipo: '{cap.TipoCapacitacion}' | EsPedagogica: {cap.EsPedagogica} | Horas: {cap.HorasAcademicas}");
                }
                
                var horasPedagogicas = capacitacionesRecientes
                    .Where(c => c.EsPedagogica || 
                               c.TipoCapacitacion.Contains("Pedagogica", StringComparison.OrdinalIgnoreCase) || 
                               c.TipoCapacitacion.Contains("Didáctica", StringComparison.OrdinalIgnoreCase) ||
                               c.TipoCapacitacion.Contains("Docencia", StringComparison.OrdinalIgnoreCase))
                    .Sum(c => c.HorasAcademicas);
                    
                Console.WriteLine($"DEBUG VerificarCapacitacionDinamica - Resultado:");
                Console.WriteLine($"  - Horas totales: {horasTotales}");
                Console.WriteLine($"  - Horas pedagógicas: {horasPedagogicas}");
                Console.WriteLine($"  - Capacitaciones pedagógicas encontradas: {capacitacionesRecientes.Count(c => c.EsPedagogica)}");
                Console.WriteLine($"  - Capacitaciones con tipo pedagógico en nombre: {capacitacionesRecientes.Count(c => c.TipoCapacitacion.Contains("Pedagógica", StringComparison.OrdinalIgnoreCase))}");

                // Verificar si cumple los requisitos
                var cumpleHoras = horasTotales >= config.HorasCapacitacionRequeridas;
                var cumplePedagogicas = horasPedagogicas >= config.HorasCapacitacionPedagogicas;
                var cumpleRequisito = cumpleHoras && cumplePedagogicas;

                string mensaje;
                if (cumpleRequisito)
                {
                    mensaje = $"✅ Cumple capacitación: {horasTotales}h totales ({horasPedagogicas}h pedagógicas)";
                }
                else
                {
                    mensaje = $"❌ No cumple capacitación: {horasTotales}h totales ({horasPedagogicas}h pedagógicas)";
                }

                return new RequisitoCumplimientoDto
                {
                    Cumple = cumpleRequisito,
                    Mensaje = mensaje,
                    ValorObtenido = $"{horasTotales}h totales ({horasPedagogicas}h pedagógicas)",
                    ValorRequerido = $"{config.HorasCapacitacionRequeridas}h totales ({config.HorasCapacitacionPedagogicas}h pedagógicas)"
                };
            }
            catch (Exception ex)
            {
                return new RequisitoCumplimientoDto
                {
                    Cumple = false,
                    Mensaje = $"❌ Error al verificar capacitación: {ex.Message}",
                    ValorObtenido = "Error",
                    ValorRequerido = $"{config.HorasCapacitacionRequeridas}h totales ({config.HorasCapacitacionPedagogicas}h pedagógicas)"
                };
            }
        }

        private async Task<RequisitoCumplimientoDto> VerificarProyectosInvestigacionDinamica(string cedula, RequisitoEscalafonConfigDto config)
        {
            try
            {
                // NOTA: Esta implementación calcula los meses de participación desde la investigación 
                // con filiación UTA más antigua, asumiendo participación continua en proyectos
                
                var investigaciones = await GetInvestigacionesDisponiblesPorCedula(cedula);
                
                Console.WriteLine($"DEBUG VerificarProyectosInvestigacionDinamica:");
                Console.WriteLine($"  - Total investigaciones: {investigaciones.Count}");
                Console.WriteLine($"  - Meses requeridos: {config.MesesProyectosInvestigacion}");
                
                // Filtrar investigaciones con filiación UTA
                var investigacionesUTA = investigaciones.Where(i => 
                    i.Filiacion.Contains("UTA", StringComparison.OrdinalIgnoreCase) ||
                    i.Filiacion.Contains("Universidad Técnica de Ambato", StringComparison.OrdinalIgnoreCase))
                    .ToList();
                
                Console.WriteLine($"  - Investigaciones con filiación UTA: {investigacionesUTA.Count}");
                
                int mesesEstimados = 0;
                DateTime? fechaInicioParticipacion = null;
                
                if (investigacionesUTA.Count > 0)
                {
                    // Buscar la investigación con filiación UTA más antigua
                    var investigacionMasAntigua = investigacionesUTA
                        .OrderBy(i => i.FechaPublicacion)
                        .First();
                    
                    fechaInicioParticipacion = investigacionMasAntigua.FechaPublicacion;
                    
                    // Calcular meses desde la investigación más antigua hasta ahora
                    var tiempoParticipacion = DateTime.Now - fechaInicioParticipacion.Value;
                    mesesEstimados = (int)(tiempoParticipacion.TotalDays / 30.44); // Promedio de días por mes
                    
                    Console.WriteLine($"  - Investigación UTA más antigua: '{investigacionMasAntigua.Titulo}' ({fechaInicioParticipacion.Value:dd/MM/yyyy})");
                    Console.WriteLine($"  - Meses desde investigación más antigua: {mesesEstimados}");
                    Console.WriteLine($"  - Años equivalentes: {mesesEstimados / 12.0:F1}");
                }
                else
                {
                    Console.WriteLine($"  - No hay investigaciones con filiación UTA registradas");
                }
                
                var cumple = mesesEstimados >= config.MesesProyectosInvestigacion;

                string mensaje;
                if (investigacionesUTA.Count > 0 && fechaInicioParticipacion.HasValue)
                {
                    mensaje = cumple 
                        ? $"✅ Cumple proyectos de investigación: {mesesEstimados} meses desde {fechaInicioParticipacion.Value:dd/MM/yyyy}"
                        : $"❌ No cumple proyectos de investigación: {mesesEstimados}/{config.MesesProyectosInvestigacion} meses desde {fechaInicioParticipacion.Value:dd/MM/yyyy}";
                }
                else
                {
                    mensaje = $"❌ No cumple proyectos de investigación: Sin investigaciones con filiación UTA registradas";
                }

                return new RequisitoCumplimientoDto
                {
                    Cumple = cumple,
                    Mensaje = mensaje,
                    ValorObtenido = investigacionesUTA.Count > 0 && fechaInicioParticipacion.HasValue
                        ? $"{mesesEstimados} meses estimados desde la investigación UTA más antigua ({fechaInicioParticipacion.Value:dd/MM/yyyy})"
                        : "0 meses - sin investigaciones con filiación UTA",
                    ValorRequerido = $"{config.MesesProyectosInvestigacion} meses mínimos en proyectos de investigación/vinculación"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR VerificarProyectosInvestigacionDinamica: {ex.Message}");
                return new RequisitoCumplimientoDto
                {
                    Cumple = false,
                    Mensaje = $"❌ Error al verificar proyectos de investigación: {ex.Message}",
                    ValorObtenido = "Error",
                    ValorRequerido = $"{config.MesesProyectosInvestigacion} meses mínimos en proyectos de investigación/vinculación"
                };
            }
        }
    }

    // DTOs para investigaciones
    public class InvestigacionDto
    {
        public int Id { get; set; }
        public string Cedula { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string RevistaOEditorial { get; set; } = string.Empty;
        public DateTime FechaPublicacion { get; set; }
        public string CampoConocimiento { get; set; } = string.Empty;
        public string Filiacion { get; set; } = string.Empty;
        public string Observacion { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool TienePdf { get; set; }
    }

    public class CreateInvestigacionDto
    {
        public string Cedula { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string RevistaOEditorial { get; set; } = string.Empty;
        public DateTime FechaPublicacion { get; set; }
        public string CampoConocimiento { get; set; } = string.Empty;
        public string Filiacion { get; set; } = string.Empty;
        public string Observacion { get; set; } = string.Empty;
    }    public class UpdateInvestigacionDto
    {
        public int Id { get; set; }
        public string Cedula { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string RevistaOEditorial { get; set; } = string.Empty;
        public DateTime FechaPublicacion { get; set; }
        public string CampoConocimiento { get; set; } = string.Empty;
        public string Filiacion { get; set; } = string.Empty;
        public string Observacion { get; set; } = string.Empty;
    }

    public class UpdateInvestigacionWithPdfDto
    {
        public int Id { get; set; }
        public string Cedula { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string RevistaOEditorial { get; set; } = string.Empty;
        public DateTime FechaPublicacion { get; set; }
        public string CampoConocimiento { get; set; } = string.Empty;
        public string Filiacion { get; set; } = string.Empty;
        public string Observacion { get; set; } = string.Empty;        public IBrowserFile? ArchivoPdf { get; set; }
    }

    // DTOs para evaluaciones de desempeño
    public class EvaluacionDesempenoDto
    {
        public int Id { get; set; }
        public string Cedula { get; set; } = string.Empty;
        public string PeriodoAcademico { get; set; } = string.Empty;
        public int Anio { get; set; }
        public int Semestre { get; set; }
        public decimal PuntajeObtenido { get; set; }
        public decimal PuntajeMaximo { get; set; } = 100;
        public decimal PorcentajeObtenido { get; set; }
        public bool CumpleMinimo { get; set; }
        public DateTime FechaEvaluacion { get; set; }
        public string TipoEvaluacion { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? Evaluador { get; set; }
        public string? NombreArchivoRespaldo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool TienePdf { get; set; }
    }

    public class CreateEvaluacionDesempenoDto
    {
        public string Cedula { get; set; } = string.Empty;
        public string PeriodoAcademico { get; set; } = string.Empty;
        public int Anio { get; set; }
        public int Semestre { get; set; }
        public decimal PuntajeObtenido { get; set; }
        public decimal PuntajeMaximo { get; set; } = 100;
        public DateTime FechaEvaluacion { get; set; }
        public string TipoEvaluacion { get; set; } = "Integral";
        public string? Observaciones { get; set; }
        public string Estado { get; set; } = "Completada";
        public string? Evaluador { get; set; }
    }

    public class CreateEvaluacionDesempenoWithPdfDto
    {
        public string Cedula { get; set; } = string.Empty;
        public string PeriodoAcademico { get; set; } = string.Empty;
        public int Anio { get; set; }
        public int Semestre { get; set; }
        public decimal PuntajeObtenido { get; set; }
        public decimal PuntajeMaximo { get; set; } = 100;
        public DateTime FechaEvaluacion { get; set; }
        public string TipoEvaluacion { get; set; } = "Integral";
        public string? Observaciones { get; set; }
        public string Estado { get; set; } = "Completada";
        public string? Evaluador { get; set; }
        public IBrowserFile? ArchivoPdf { get; set; }
    }

    public class UpdateEvaluacionDesempenoDto
    {
        public int Id { get; set; }
        public string Cedula { get; set; } = string.Empty;
        public string PeriodoAcademico { get; set; } = string.Empty;
        public int Anio { get; set; }
        public int Semestre { get; set; }
        public decimal PuntajeObtenido { get; set; }
        public decimal PuntajeMaximo { get; set; } = 100;
        public DateTime FechaEvaluacion { get; set; }
        public string TipoEvaluacion { get; set; } = "Integral";
        public string? Observaciones { get; set; }
        public string Estado { get; set; } = "Completada";
        public string? Evaluador { get; set; }
    }

    public class UpdateEvaluacionDesempenoWithPdfDto
    {
        public int Id { get; set; }
        public string Cedula { get; set; } = string.Empty;
        public string PeriodoAcademico { get; set; } = string.Empty;
        public int Anio { get; set; }
        public int Semestre { get; set; }
        public decimal PuntajeObtenido { get; set; }
        public decimal PuntajeMaximo { get; set; } = 100;
        public DateTime FechaEvaluacion { get; set; }
        public string TipoEvaluacion { get; set; } = "Integral";
        public string? Observaciones { get; set; }
        public string Estado { get; set; } = "Completada";
        public string? Evaluador { get; set; }
        public IBrowserFile? ArchivoPdf { get; set; }
    }

public class RegisterRequest
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
        public bool IsAccountLocked { get; set; } = false;    }    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int UserType { get; set; }
        public string Cedula { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        
        [JsonPropertyName("nivel")]
        public string? Nivel { get; set; } // Agregado para exponer el nivel
        public string FullName => $"{FirstName} {LastName}";
    }public class CheckEmailResponse
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
    }public class CreateInvestigacionWithPdfDto
    {
        public string Cedula { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string RevistaOEditorial { get; set; } = string.Empty;
        public DateTime FechaPublicacion { get; set; }
        public string CampoConocimiento { get; set; } = string.Empty;
        public string Filiacion { get; set; } = string.Empty;
        public string Observacion { get; set; } = string.Empty;
        public IBrowserFile? ArchivoPdf { get; set; }
    }

    // DTOs para verificación de requisitos para subir de nivel
    public class VerificacionRequisitosSubirNivelDto
    {
        public string Cedula { get; set; } = string.Empty;
        public bool CumpleTodosRequisitos { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public RequisitoCumplimientoDto Experiencia { get; set; } = new();
        public RequisitoCumplimientoDto ObraRelevante { get; set; } = new();
        public RequisitoCumplimientoDto Evaluacion75Porciento { get; set; } = new();
        public RequisitoCumplimientoDto Capacitacion96Horas { get; set; } = new();
        public DateTime FechaVerificacion { get; set; } = DateTime.Now;
    }

    public class RequisitoCumplimientoDto
    {
        public bool Cumple { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public string ValorObtenido { get; set; } = string.Empty;
        public string ValorRequerido { get; set; } = string.Empty;    }

    // DTOs para respuestas de APIs
    public class VerificacionRequisitoDiticResponse
    {
        public string Cedula { get; set; } = string.Empty;
        public bool CumpleRequisito { get; set; }
        public bool CumpleHorasTotales { get; set; }
        public bool CumpleHorasPedagogicas { get; set; }
        public bool TieneExencionAutoridad { get; set; }
        public int HorasRequeridas { get; set; } = 96;
        public int HorasPedagogicasRequeridas { get; set; } = 24;
        public int HorasObtenidas { get; set; }
        public int HorasPedagogicasObtenidas { get; set; }
        public decimal PorcentajePedagogico { get; set; }
        public int CapacitacionesAnalizadas { get; set; }
        public string MensajeDetallado { get; set; } = string.Empty;
        public string? CargoAutoridad { get; set; }
        public decimal? AñosComoAutoridad { get; set; }
    }

    // DTO para TTHH
    public class TTHHDto
    {
        public int Id { get; set; }
        public string Cedula { get; set; } = string.Empty;        public DateTime FechaInicio { get; set; }
        public double AniosCumplidos { get; set; }
        public string Observacion { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    // DTOs para estadísticas del docente
    public class EstadisticasDocenteResponse
    {
        public string Cedula { get; set; } = string.Empty;
        public DateTime FechaConsulta { get; set; }
        public ResumenEstadisticas Resumen { get; set; } = new();
        public SeccionesEstadisticas Secciones { get; set; } = new();
    }

    public class ResumenEstadisticas
    {
        public int TotalRequisitos { get; set; }
        public int RequisitosCumplidos { get; set; }
        public double PorcentajeCompletitud { get; set; }
        public bool PuedeSubirNivel { get; set; }
    }

    public class SeccionesEstadisticas
    {
        public SeccionEstadistica Experiencia { get; set; } = new();
        public SeccionEstadistica Obras { get; set; } = new();
        public SeccionEstadistica Evaluaciones { get; set; } = new();
        public SeccionEstadistica Capacitaciones { get; set; } = new();
        public SeccionEstadistica Proyectos { get; set; } = new();
    }

    public class SeccionEstadistica
    {
        public string Titulo { get; set; } = string.Empty;
        public string Icono { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public DatosSeccion Datos { get; set; } = new();
    }

    public class DatosSeccion
    {
        // Experiencia
        public int AñosRequeridos { get; set; }
        public double AñosObtenidos { get; set; }
        
        // Obras
        public int TotalObras { get; set; }
        public int ObrasConUTA { get; set; }
        
        // Evaluaciones
        public int EvaluacionesAnalizadas { get; set; }
        public decimal PromedioObtenido { get; set; }
        public decimal Requiere75 { get; set; }
        
        // Capacitaciones
        public int HorasRequeridas { get; set; }
        public int HorasObtenidas { get; set; }
        public int HorasPedagogicasRequeridas { get; set; }
        public int HorasPedagogicasObtenidas { get; set; }
        
        // Proyectos de Investigación
        public int MesesRequeridos { get; set; }
        public int MesesObtenidos { get; set; }
        public int ProyectosActivos { get; set; }
        
        // Común
        public bool Cumple { get; set; }
        public string Detalles { get; set; } = string.Empty;
    }

    // DTOs específicos para AuthService que usan IBrowserFile
    public class CreateDiticWithPdfDto
    {
        public string Cedula { get; set; } = string.Empty;
        public string NombreCapacitacion { get; set; } = string.Empty;
        public string Institucion { get; set; } = string.Empty;
        public string TipoCapacitacion { get; set; } = string.Empty;
        public string Modalidad { get; set; } = "Presencial";
        public int HorasAcademicas { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int Anio { get; set; }
        public string Estado { get; set; } = "Completada";
        public decimal? Calificacion { get; set; }
        public decimal CalificacionMinima { get; set; } = 70;
        public string? Descripcion { get; set; }
        public string? NumeroCertificado { get; set; }
        public string? Instructor { get; set; }
        public string? Observaciones { get; set; }
        public bool ExencionPorAutoridad { get; set; } = false;
        public string? CargoAutoridad { get; set; }
        public DateTime? FechaInicioAutoridad { get; set; }
        public DateTime? FechaFinAutoridad { get; set; }
        public IBrowserFile? ArchivoCertificado { get; set; }
    }

    // Alias del DTO para capacitaciones DITIC compatible con AuthService
    public class DiticDto
    {
        public int Id { get; set; }
        public string Cedula { get; set; } = string.Empty;
        public string NombreCapacitacion { get; set; } = string.Empty;
        public string Institucion { get; set; } = string.Empty;
        public string TipoCapacitacion { get; set; } = string.Empty;
        public string Modalidad { get; set; } = string.Empty;
        public int HorasAcademicas { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int Anio { get; set; }
        public string Estado { get; set; } = string.Empty;
        public decimal? Calificacion { get; set; }
        public decimal CalificacionMinima { get; set; }
        public bool Aprobada { get; set; }
        public bool EsPedagogica { get; set; }
        public string? Descripcion { get; set; }
        public string? NumeroCertificado { get; set; }
        public string? Instructor { get; set; }
        public string? Observaciones { get; set; }
        public string? NombreArchivoCertificado { get; set; }
        public bool ExencionPorAutoridad { get; set; }
        public string? CargoAutoridad { get; set; }
        public DateTime? FechaInicioAutoridad { get; set; }
        public DateTime? FechaFinAutoridad { get; set; }
        public decimal AñosComoAutoridad { get; set; }
        public bool CumpleExencionAutoridad { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    // DTO para crear solicitudes de escalafón
    public class CreateSolicitudEscalafonDto
    {
        public string DocenteCedula { get; set; } = string.Empty;
        public string DocenteNombre { get; set; } = string.Empty;
        public string DocenteEmail { get; set; } = string.Empty;
        public string? DocenteTelefono { get; set; }
        public string? Facultad { get; set; }
        public string? Carrera { get; set; }
        public string NivelActual { get; set; } = string.Empty;
        public string NivelSolicitado { get; set; } = string.Empty;
        public int AnosExperiencia { get; set; }
        public string? Titulos { get; set; }
        public string? Publicaciones { get; set; }
        public string? ProyectosInvestigacion { get; set; }
        public string? Capacitaciones { get; set; }
        public string? Observaciones { get; set; }
    }

    // DTO para respuesta de verificación de evaluación de desempeño
    public class VerificacionRequisitoEvaluacionResponse
    {
        public string Cedula { get; set; } = string.Empty;
        public bool CumpleRequisito75Porciento { get; set; }
        public decimal PorcentajePromedioUltimasCuatro { get; set; }
        public int EvaluacionesAnalizadas { get; set; }
        public string MensajeDetallado { get; set; } = string.Empty;
        public List<EvaluacionDesempenoSimpleDto> EvaluacionesConsideradas { get; set; } = new();
    }

    public class EvaluacionDesempenoSimpleDto
    {
        public string PeriodoAcademico { get; set; } = string.Empty;
        public int Anio { get; set; }
        public string Semestre { get; set; } = string.Empty;
        public decimal PuntajeObtenido { get; set; }
        public decimal PuntajeMaximo { get; set; }
        public decimal PorcentajeObtenido { get; set; }
        public DateTime FechaEvaluacion { get; set; }
    }
}
