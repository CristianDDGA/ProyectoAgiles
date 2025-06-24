using ProyectoAgiles.Domain.Entities;

namespace ProyectoAgiles.Domain.Interfaces;

/// <summary>
/// Interfaz para el repositorio de evaluaciones de desempeño
/// </summary>
public interface IEvaluacionDesempenoRepository : IRepository<EvaluacionDesempeno>
{
    /// <summary>
    /// Obtiene todas las evaluaciones de un docente por cédula
    /// </summary>
    Task<IEnumerable<EvaluacionDesempeno>> GetByCedulaAsync(string cedula);

    /// <summary>
    /// Obtiene las últimas 4 evaluaciones de un docente por cédula
    /// </summary>
    Task<IEnumerable<EvaluacionDesempeno>> GetUltimasCuatroEvaluacionesByCedulaAsync(string cedula);

    /// <summary>
    /// Obtiene evaluaciones por período académico
    /// </summary>
    Task<IEnumerable<EvaluacionDesempeno>> GetByPeriodoAcademicoAsync(string periodoAcademico);

    /// <summary>
    /// Obtiene evaluaciones por año
    /// </summary>
    Task<IEnumerable<EvaluacionDesempeno>> GetByAnioAsync(int anio);

    /// <summary>
    /// Obtiene evaluaciones por año y semestre
    /// </summary>
    Task<IEnumerable<EvaluacionDesempeno>> GetByAnioAndSemestreAsync(int anio, int semestre);

    /// <summary>
    /// Obtiene evaluaciones que cumplen con el mínimo requerido (75%)
    /// </summary>
    Task<IEnumerable<EvaluacionDesempeno>> GetEvaluacionesQueAlcanzan75PorCientoAsync();

    /// <summary>
    /// Obtiene evaluaciones que cumplen con el mínimo requerido por cédula
    /// </summary>
    Task<IEnumerable<EvaluacionDesempeno>> GetEvaluacionesQueAlcanzan75PorCientoByCedulaAsync(string cedula);

    /// <summary>
    /// Verifica si un docente cumple con el requisito del 75% en las últimas 4 evaluaciones
    /// </summary>
    Task<bool> CumpleRequisito75PorCientoAsync(string cedula);

    /// <summary>
    /// Obtiene el promedio de las últimas 4 evaluaciones de un docente
    /// </summary>
    Task<decimal> GetPromedioUltimasCuatroEvaluacionesAsync(string cedula);

    /// <summary>
    /// Verifica si existe una evaluación para un período específico y cédula
    /// </summary>
    Task<bool> ExisteEvaluacionParaPeriodoAsync(string cedula, string periodoAcademico);

    /// <summary>
    /// Obtiene el archivo PDF de respaldo por ID
    /// </summary>
    Task<byte[]?> GetArchivoPdfByIdAsync(int id);

    /// <summary>
    /// Obtiene estadísticas generales de evaluaciones
    /// </summary>
    Task<object> GetEstadisticasGeneralesAsync();

    /// <summary>
    /// Método de prueba para verificar conexión a tabla DAC
    /// </summary>
    Task<object> TestDacConnectionAsync();
}
