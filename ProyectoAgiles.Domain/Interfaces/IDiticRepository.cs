using ProyectoAgiles.Domain.Entities;

namespace ProyectoAgiles.Domain.Interfaces;

/// <summary>
/// Interfaz para el repositorio de capacitaciones DITIC
/// </summary>
public interface IDiticRepository
{
    // Operaciones CRUD básicas
    Task<DITIC?> GetByIdAsync(int id);
    Task<IEnumerable<DITIC>> GetAllAsync();
    Task<DITIC> CreateAsync(DITIC ditic);
    Task<DITIC> UpdateAsync(DITIC ditic);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);

    // Consultas específicas por cédula
    Task<IEnumerable<DITIC>> GetByCedulaAsync(string cedula);
    Task<IEnumerable<DITIC>> GetByCedulaAndYearRangeAsync(string cedula, int añoInicio, int añoFin);
    Task<IEnumerable<DITIC>> GetByCedulaLastThreeYearsAsync(string cedula);

    // Consultas para verificación de requisitos
    Task<IEnumerable<DITIC>> GetApprovedByCedulaAsync(string cedula);
    Task<IEnumerable<DITIC>> GetApprovedByCedulaLastThreeYearsAsync(string cedula);
    Task<IEnumerable<DITIC>> GetPedagogicalByCedulaLastThreeYearsAsync(string cedula);

    // Estadísticas y reportes
    Task<int> GetTotalHoursByCedulaAsync(string cedula);
    Task<int> GetTotalHoursByCedulaLastThreeYearsAsync(string cedula);
    Task<int> GetPedagogicalHoursByCedulaLastThreeYearsAsync(string cedula);
    Task<int> GetCountByCedulaAsync(string cedula);

    // Verificación de exención por autoridad
    Task<DITIC?> GetAuthorityExemptionByCedulaAsync(string cedula);
    Task<bool> HasAuthorityExemptionAsync(string cedula);

    // Consultas por filtros
    Task<IEnumerable<DITIC>> GetByTypeAsync(string tipoCapacitacion);
    Task<IEnumerable<DITIC>> GetByInstitutionAsync(string institucion);
    Task<IEnumerable<DITIC>> GetByYearAsync(int año);
    Task<IEnumerable<DITIC>> GetByStatusAsync(string estado);

    // Búsqueda avanzada
    Task<IEnumerable<DITIC>> SearchAsync(string? searchTerm, string? cedula = null, 
                                        string? tipo = null, string? institucion = null, 
                                        int? año = null, string? estado = null);

    // Importación de datos externos
    Task<int> ImportFromExternalSystemAsync(string cedula);
    Task<bool> ValidateExternalDataAsync(string cedula);
}
