using ProyectoAgiles.Domain.Entities;

namespace ProyectoAgiles.Domain.Interfaces;

public interface IPeriodoPostulacionRepository
{
    Task<IEnumerable<PeriodoPostulacion>> GetAllAsync();
    Task<PeriodoPostulacion?> GetByIdAsync(int id);
    Task<PeriodoPostulacion?> GetActivePeriodAsync();
    Task<PeriodoPostulacion> CreateAsync(PeriodoPostulacion periodo);
    Task<PeriodoPostulacion> UpdateAsync(PeriodoPostulacion periodo);
    Task<bool> DeleteAsync(int id);
    Task<bool> ActivatePeriodAsync(int id);
    Task<bool> DeactivatePeriodAsync(int id);
    Task<bool> DeactivateAllPeriodsAsync();
    Task<IEnumerable<PeriodoPostulacion>> GetPeriodsByDateRangeAsync(DateTime startDate, DateTime endDate);
}
