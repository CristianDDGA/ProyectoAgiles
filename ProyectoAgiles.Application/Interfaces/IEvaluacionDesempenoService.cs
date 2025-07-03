using ProyectoAgiles.Application.DTOs;

namespace ProyectoAgiles.Application.Interfaces;

/// <summary>
/// Interfaz para el servicio de evaluaciones de desempeño
/// </summary>
public interface IEvaluacionDesempenoService
{
    /// <summary>
    /// Obtiene todas las evaluaciones
    /// </summary>
    Task<IEnumerable<EvaluacionDesempenoDto>> GetAllAsync();

    /// <summary>
    /// Obtiene una evaluación por ID
    /// </summary>
    Task<EvaluacionDesempenoDto?> GetByIdAsync(int id);

    /// <summary>
    /// Obtiene evaluaciones por cédula del docente
    /// </summary>
    Task<IEnumerable<EvaluacionDesempenoDto>> GetByCedulaAsync(string cedula);

    /// <summary>
    /// Obtiene evaluaciones disponibles (no utilizadas) para escalafón
    /// </summary>
    Task<IEnumerable<EvaluacionDesempenoDto>> GetDisponiblesParaEscalafonAsync(string cedula);

    /// <summary>
    /// Obtiene las últimas 4 evaluaciones de un docente
    /// </summary>
    Task<IEnumerable<EvaluacionDesempenoDto>> GetUltimasCuatroEvaluacionesByCedulaAsync(string cedula);

    /// <summary>
    /// Obtiene evaluaciones por período académico
    /// </summary>
    Task<IEnumerable<EvaluacionDesempenoDto>> GetByPeriodoAcademicoAsync(string periodoAcademico);

    /// <summary>
    /// Obtiene evaluaciones por año
    /// </summary>
    Task<IEnumerable<EvaluacionDesempenoDto>> GetByAnioAsync(int anio);

    /// <summary>
    /// Obtiene evaluaciones por año y semestre
    /// </summary>
    Task<IEnumerable<EvaluacionDesempenoDto>> GetByAnioAndSemestreAsync(int anio, int semestre);

    /// <summary>
    /// Crea una nueva evaluación
    /// </summary>
    Task<EvaluacionDesempenoDto> CreateAsync(CreateEvaluacionDesempenoDto createDto);

    /// <summary>
    /// Crea una nueva evaluación con archivo PDF
    /// </summary>
    Task<EvaluacionDesempenoDto> CreateWithPdfAsync(CreateEvaluacionWithPdfDto createDto);

    /// <summary>
    /// Actualiza una evaluación existente
    /// </summary>
    Task<EvaluacionDesempenoDto> UpdateAsync(UpdateEvaluacionDesempenoDto updateDto);

    /// <summary>
    /// Actualiza una evaluación existente con archivo PDF
    /// </summary>
    Task<EvaluacionDesempenoDto> UpdateWithPdfAsync(UpdateEvaluacionWithPdfDto updateDto);

    /// <summary>
    /// Elimina una evaluación (soft delete)
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Verifica si existe una evaluación
    /// </summary>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Obtiene el resumen de evaluaciones de un docente
    /// </summary>
    Task<ResumenEvaluacionesDto> GetResumenEvaluacionesByCedulaAsync(string cedula);

    /// <summary>
    /// Verifica si un docente cumple con el requisito del 75%
    /// </summary>
    Task<VerificacionRequisito75Dto> VerificarRequisito75PorCientoAsync(string cedula);

    /// <summary>
    /// Obtiene evaluaciones que alcanzan el 75%
    /// </summary>
    Task<IEnumerable<EvaluacionDesempenoDto>> GetEvaluacionesQueAlcanzan75PorCientoAsync();

    /// <summary>
    /// Obtiene evaluaciones que alcanzan el 75% por cédula
    /// </summary>
    Task<IEnumerable<EvaluacionDesempenoDto>> GetEvaluacionesQueAlcanzan75PorCientoByCedulaAsync(string cedula);

    /// <summary>
    /// Obtiene el archivo PDF por ID
    /// </summary>
    Task<byte[]?> GetPdfByIdAsync(int id);

    /// <summary>
    /// Obtiene estadísticas generales
    /// </summary>
    Task<object> GetEstadisticasGeneralesAsync();

    /// <summary>
    /// Verifica si existe evaluación para un período específico
    /// </summary>
    Task<bool> ExisteEvaluacionParaPeriodoAsync(string cedula, string periodoAcademico);

    /// <summary>
    /// Método de prueba para verificar conexión a tabla DAC
    /// </summary>
    Task<object> TestDacConnectionAsync();
}
