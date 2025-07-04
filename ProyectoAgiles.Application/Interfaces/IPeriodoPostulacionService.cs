using ProyectoAgiles.Application.DTOs;

namespace ProyectoAgiles.Application.Interfaces;

public interface IPeriodoPostulacionService
{
    Task<ApiResponse<IEnumerable<PeriodoPostulacionDto>>> GetAllPeriodsAsync();
    Task<ApiResponse<PeriodoPostulacionDto>> GetPeriodByIdAsync(int id);
    Task<ApiResponse<PeriodoInfoDto>> GetActivePeriodInfoAsync();
    Task<ApiResponse<PeriodoPostulacionDto>> CreatePeriodAsync(CreatePeriodoPostulacionDto createDto);
    Task<ApiResponse<PeriodoPostulacionDto>> UpdatePeriodAsync(int id, UpdatePeriodoPostulacionDto updateDto);
    Task<ApiResponse<bool>> DeletePeriodAsync(int id);
    Task<ApiResponse<bool>> ActivatePeriodAsync(int id);
    Task<ApiResponse<bool>> DeactivatePeriodAsync(int id);
    Task<ApiResponse<bool>> DeactivateAllPeriodsAsync();
    Task<ApiResponse<bool>> ValidateCanCreateSolicitudAsync();
    Task<ApiResponse<IEnumerable<PeriodoPostulacionDto>>> GetPeriodsByDateRangeAsync(DateTime startDate, DateTime endDate);
}
