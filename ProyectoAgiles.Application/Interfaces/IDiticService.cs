using ProyectoAgiles.Application.DTOs;

namespace ProyectoAgiles.Application.Interfaces;

/// <summary>
/// Interfaz para el servicio de capacitaciones DITIC
/// </summary>
public interface IDiticService
{
    // Operaciones CRUD básicas
    Task<DiticDto?> GetByIdAsync(int id);
    Task<IEnumerable<DiticDto>> GetAllAsync();
    Task<DiticDto> CreateAsync(CreateDiticDto createDto);
    Task<DiticDto> CreateWithPdfAsync(CreateDiticWithPdfDto createDto);
    Task<DiticDto> UpdateAsync(UpdateDiticDto updateDto);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);

    // Consultas específicas por cédula
    Task<IEnumerable<DiticDto>> GetByCedulaAsync(string cedula);
    Task<IEnumerable<DiticDto>> GetDisponiblesParaEscalafonAsync(string cedula);
    Task<IEnumerable<DiticDto>> GetByCedulaLastThreeYearsAsync(string cedula);

    // Verificación de requisitos DITIC
    Task<VerificacionRequisitoDiticDto> VerifyRequirementAsync(string cedula);
    Task<ResumenCapacitacionesDto> GetSummaryByCedulaAsync(string cedula);

    // Estadísticas y reportes
    Task<IEnumerable<EstadisticasCapacitacionDto>> GetStatisticsByTypeAsync(string cedula);
    Task<int> GetTotalHoursAsync(string cedula);
    Task<int> GetTotalHoursLastThreeYearsAsync(string cedula);
    Task<int> GetPedagogicalHoursLastThreeYearsAsync(string cedula);

    // Gestión de archivos PDF
    Task<byte[]?> GetCertificatePdfAsync(int id);
    Task<bool> UpdateCertificatePdfAsync(int id, byte[] pdfContent, string fileName);
    Task<bool> DeleteCertificatePdfAsync(int id);

    // Importación de datos externos
    Task<int> ImportFromExternalSystemAsync(string cedula);
    Task<bool> ValidateExternalDataAsync(string cedula);

    // Búsqueda avanzada
    Task<IEnumerable<DiticDto>> SearchAsync(string? searchTerm, string? cedula = null, 
                                           string? tipo = null, string? institucion = null, 
                                           int? año = null, string? estado = null);

    // Validaciones de negocio
    Task<bool> ValidateTrainingHoursAsync(CreateDiticDto createDto);
    Task<bool> ValidateDateRangeAsync(DateTime fechaInicio, DateTime fechaFin);
    Task<bool> ValidateAuthorityExemptionAsync(CreateDiticDto createDto);
}
