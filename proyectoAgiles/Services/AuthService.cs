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
                var investigaciones = await GetInvestigacionesPorCedula(cedula);
                
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

        public async Task<EstadisticasDocenteResponse> ObtenerEstadisticasDocente(string cedula)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<EstadisticasDocenteResponse>($"{_apiBaseUrl}/api/EvaluacionesDesempeno/estadisticas-docente/{cedula}");
                return response ?? new EstadisticasDocenteResponse
                {
                    Cedula = cedula,
                    Resumen = new ResumenEstadisticas { TotalRequisitos = 4, RequisitosCumplidos = 0, PorcentajeCompletitud = 0, PuedeSubirNivel = false }
                };
            }
            catch (Exception)
            {
                return new EstadisticasDocenteResponse
                {
                    Cedula = cedula,
                    Resumen = new ResumenEstadisticas { TotalRequisitos = 4, RequisitosCumplidos = 0, PorcentajeCompletitud = 0, PuedeSubirNivel = false }
                };
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

        public async Task<bool> ActualizarCertificadoCapacitacion(int id, IBrowserFile archivoCertificado)
        {
            try
            {
                using var form = new MultipartFormDataContent();
                if (archivoCertificado != null)
                {
                    var stream = archivoCertificado.OpenReadStream(10 * 1024 * 1024); // 10MB máx
                    var pdfContent = new StreamContent(stream);
                    pdfContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                    form.Add(pdfContent, "archivo", archivoCertificado.Name);
                }
                else
                {
                    throw new Exception("No se seleccionó un archivo PDF para actualizar");
                }
                var response = await _httpClient.PutAsync($"{_apiBaseUrl}/api/ditic/{id}/certificado", form);
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar el certificado PDF: {ex.Message}");
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
}
