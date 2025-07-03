using System.Text.Json;

namespace proyectoAgiles.Services;

public class PeriodoPostulacionService
{
    private readonly HttpClient _httpClient;

    public PeriodoPostulacionService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResponse<IEnumerable<PeriodoPostulacionDto>>> GetAllPeriodsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/PeriodosPostulacion");
            var jsonResponse = await response.Content.ReadAsStringAsync();
            
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<IEnumerable<PeriodoPostulacionDto>>>(
                jsonResponse, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            return apiResponse ?? new ApiResponse<IEnumerable<PeriodoPostulacionDto>>
            {
                Success = false,
                Message = "Error al deserializar la respuesta"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<IEnumerable<PeriodoPostulacionDto>>
            {
                Success = false,
                Message = $"Error de conexión: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<PeriodoPostulacionDto>> GetPeriodByIdAsync(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/PeriodosPostulacion/{id}");
            var jsonResponse = await response.Content.ReadAsStringAsync();
            
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<PeriodoPostulacionDto>>(
                jsonResponse, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            return apiResponse ?? new ApiResponse<PeriodoPostulacionDto>
            {
                Success = false,
                Message = "Error al deserializar la respuesta"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<PeriodoPostulacionDto>
            {
                Success = false,
                Message = $"Error de conexión: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<PeriodoInfoFrontendDto>> GetActivePeriodInfoAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/PeriodosPostulacion/active-info");
            var jsonResponse = await response.Content.ReadAsStringAsync();
            
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<PeriodoInfoDto>>(
                jsonResponse, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            if (apiResponse?.Success == true && apiResponse.Data != null)
            {
                // Convertir la respuesta del backend al formato del frontend
                var frontendDto = new PeriodoInfoFrontendDto
                {
                    HayPeriodoActivo = apiResponse.Data.HayPeriodoActivo,
                    PuedeCrearSolicitud = apiResponse.Data.HayPeriodoActivo,
                    Mensaje = apiResponse.Data.Mensaje,
                    DiasRestantes = apiResponse.Data.DiasRestantes,
                    EstadoPeriodo = apiResponse.Data.EstadoPeriodo,
                    FechaInicio = apiResponse.Data.FechaInicio,
                    FechaFin = apiResponse.Data.FechaFin,
                    Descripcion = apiResponse.Data.Descripcion,
                    PeriodoActivo = apiResponse.Data.HayPeriodoActivo && apiResponse.Data.FechaInicio.HasValue && apiResponse.Data.FechaFin.HasValue ? 
                        new PeriodoPostulacionDto
                        {
                            Id = 1, // Dummy ID since the backend doesn't provide it
                            FechaInicio = apiResponse.Data.FechaInicio.Value,
                            FechaFin = apiResponse.Data.FechaFin.Value,
                            Activo = true,
                            Descripcion = apiResponse.Data.Descripcion,
                            EstadoPeriodo = apiResponse.Data.EstadoPeriodo,
                            DiasRestantes = apiResponse.Data.DiasRestantes,
                            FechaCreacion = DateTime.Now
                        } : null
                };
                
                return new ApiResponse<PeriodoInfoFrontendDto>
                {
                    Success = true,
                    Message = apiResponse.Message,
                    Data = frontendDto
                };
            }
            
            return new ApiResponse<PeriodoInfoFrontendDto>
            {
                Success = false,
                Message = apiResponse?.Message ?? "Error al deserializar la respuesta"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<PeriodoInfoFrontendDto>
            {
                Success = false,
                Message = $"Error de conexión: {ex.Message}",
                Data = new PeriodoInfoFrontendDto
                {
                    PuedeCrearSolicitud = false,
                    HayPeriodoActivo = false,
                    Mensaje = "No se pudo verificar el período de postulación",
                    DiasRestantes = 0
                }
            };
        }
    }

    public async Task<ApiResponse<bool>> ValidateCanCreateSolicitudAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/PeriodosPostulacion/validate-creation");
            var jsonResponse = await response.Content.ReadAsStringAsync();
            
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<bool>>(
                jsonResponse, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            return apiResponse ?? new ApiResponse<bool>
            {
                Success = false,
                Message = "Error al deserializar la respuesta",
                Data = false
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Message = $"Error de conexión: {ex.Message}",
                Data = false
            };
        }
    }

    public async Task<ApiResponse<PeriodoPostulacionDto>> CreatePeriodAsync(CreatePeriodoPostulacionDto createDto)
    {
        try
        {
            var jsonContent = JsonSerializer.Serialize(createDto);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/api/PeriodosPostulacion", content);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<PeriodoPostulacionDto>>(
                jsonResponse, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            return apiResponse ?? new ApiResponse<PeriodoPostulacionDto>
            {
                Success = false,
                Message = "Error al deserializar la respuesta"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<PeriodoPostulacionDto>
            {
                Success = false,
                Message = $"Error de conexión: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<PeriodoPostulacionDto>> UpdatePeriodAsync(int id, UpdatePeriodoPostulacionDto updateDto)
    {
        try
        {
            var jsonContent = JsonSerializer.Serialize(updateDto);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"/api/PeriodosPostulacion/{id}", content);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<PeriodoPostulacionDto>>(
                jsonResponse, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            return apiResponse ?? new ApiResponse<PeriodoPostulacionDto>
            {
                Success = false,
                Message = "Error al deserializar la respuesta"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<PeriodoPostulacionDto>
            {
                Success = false,
                Message = $"Error de conexión: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<bool>> ActivatePeriodAsync(int id)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/PeriodosPostulacion/{id}/activate", null);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<bool>>(
                jsonResponse, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            return apiResponse ?? new ApiResponse<bool>
            {
                Success = false,
                Message = "Error al deserializar la respuesta"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Message = $"Error de conexión: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<bool>> DeactivatePeriodAsync(int id)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/PeriodosPostulacion/{id}/deactivate", null);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<bool>>(
                jsonResponse, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            return apiResponse ?? new ApiResponse<bool>
            {
                Success = false,
                Message = "Error al deserializar la respuesta"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Message = $"Error de conexión: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<bool>> DeactivateAllPeriodsAsync()
    {
        try
        {
            var response = await _httpClient.PostAsync("/api/PeriodosPostulacion/deactivate-all", null);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<bool>>(
                jsonResponse, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            return apiResponse ?? new ApiResponse<bool>
            {
                Success = false,
                Message = "Error al deserializar la respuesta"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Message = $"Error de conexión: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<bool>> DeletePeriodAsync(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/PeriodosPostulacion/{id}");
            var jsonResponse = await response.Content.ReadAsStringAsync();
            
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<bool>>(
                jsonResponse, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            return apiResponse ?? new ApiResponse<bool>
            {
                Success = false,
                Message = "Error al deserializar la respuesta"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<bool>
            {
                Success = false,
                Message = $"Error de conexión: {ex.Message}"
            };
        }
    }
}
