using Microsoft.EntityFrameworkCore;
using ProyectoAgiles.Domain.Entities;
using ProyectoAgiles.Domain.Interfaces;
using ProyectoAgiles.Infrastructure.Data;

namespace ProyectoAgiles.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio para evaluaciones de desempeño
/// </summary>
public class EvaluacionDesempenoRepository : Repository<EvaluacionDesempeno>, IEvaluacionDesempenoRepository
{
    private readonly ApplicationDbContext _dbContext;

    public EvaluacionDesempenoRepository(ApplicationDbContext context) : base(context)
    {
        _dbContext = context;
    }    /// <summary>
    /// Obtiene todas las evaluaciones de un docente por cédula
    /// </summary>
    public async Task<IEnumerable<EvaluacionDesempeno>> GetByCedulaAsync(string cedula)
    {
        return await _dbContext.DAC
            .Where(e => e.Cedula == cedula && !e.IsDeleted)
            .OrderByDescending(e => e.Anio)
            .ThenByDescending(e => e.Semestre)
            .ToListAsync();
    }    /// <summary>
    /// Obtiene las últimas 4 evaluaciones de un docente por cédula
    /// </summary>
    public async Task<IEnumerable<EvaluacionDesempeno>> GetUltimasCuatroEvaluacionesByCedulaAsync(string cedula)
    {
        return await _dbContext.DAC
            .Where(e => e.Cedula == cedula && !e.IsDeleted)
            .OrderByDescending(e => e.Anio)
            .ThenByDescending(e => e.Semestre)
            .Take(4)
            .ToListAsync();
    }    /// <summary>
    /// Obtiene evaluaciones por período académico
    /// </summary>
    public async Task<IEnumerable<EvaluacionDesempeno>> GetByPeriodoAcademicoAsync(string periodoAcademico)
    {
        return await _dbContext.DAC
            .Where(e => e.PeriodoAcademico == periodoAcademico && !e.IsDeleted)
            .OrderBy(e => e.Cedula)
            .ToListAsync();
    }    /// <summary>
    /// Obtiene evaluaciones por año
    /// </summary>
    public async Task<IEnumerable<EvaluacionDesempeno>> GetByAnioAsync(int anio)
    {
        return await _dbContext.DAC
            .Where(e => e.Anio == anio && !e.IsDeleted)
            .OrderBy(e => e.Cedula)
            .ThenBy(e => e.Semestre)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene evaluaciones por año y semestre
    /// </summary>
    public async Task<IEnumerable<EvaluacionDesempeno>> GetByAnioAndSemestreAsync(int anio, int semestre)
    {
        return await _dbContext.DAC
            .Where(e => e.Anio == anio && e.Semestre == semestre && !e.IsDeleted)
            .OrderBy(e => e.Cedula)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene evaluaciones que cumplen con el mínimo requerido (75%)
    /// </summary>
    public async Task<IEnumerable<EvaluacionDesempeno>> GetEvaluacionesQueAlcanzan75PorCientoAsync()
    {
        return await _dbContext.DAC
            .Where(e => !e.IsDeleted && (e.PuntajeObtenido / e.PuntajeMaximo) * 100 >= 75)
            .OrderByDescending(e => e.Anio)
            .ThenByDescending(e => e.Semestre)
            .ThenBy(e => e.Cedula)
            .ToListAsync();
    }

    /// <summary>
    /// Obtiene evaluaciones que cumplen con el mínimo requerido por cédula
    /// </summary>
    public async Task<IEnumerable<EvaluacionDesempeno>> GetEvaluacionesQueAlcanzan75PorCientoByCedulaAsync(string cedula)
    {
        return await _dbContext.DAC
            .Where(e => e.Cedula == cedula && !e.IsDeleted && (e.PuntajeObtenido / e.PuntajeMaximo) * 100 >= 75)
            .OrderByDescending(e => e.Anio)
            .ThenByDescending(e => e.Semestre)
            .ToListAsync();
    }

    /// <summary>
    /// Verifica si un docente cumple con el requisito del 75% en las últimas 4 evaluaciones
    /// </summary>
    public async Task<bool> CumpleRequisito75PorCientoAsync(string cedula)
    {
        var ultimasCuatroEvaluaciones = await GetUltimasCuatroEvaluacionesByCedulaAsync(cedula);
        
        if (!ultimasCuatroEvaluaciones.Any())
            return false;

        // Debe tener al menos 4 evaluaciones y todas deben alcanzar el 75%
        return ultimasCuatroEvaluaciones.Count() >= 4 && 
               ultimasCuatroEvaluaciones.All(e => (e.PuntajeObtenido / e.PuntajeMaximo) * 100 >= 75);
    }

    /// <summary>
    /// Obtiene el promedio de las últimas 4 evaluaciones de un docente
    /// </summary>
    public async Task<decimal> GetPromedioUltimasCuatroEvaluacionesAsync(string cedula)
    {
        var ultimasCuatroEvaluaciones = await GetUltimasCuatroEvaluacionesByCedulaAsync(cedula);
        
        if (!ultimasCuatroEvaluaciones.Any())
            return 0;

        return ultimasCuatroEvaluaciones.Average(e => (e.PuntajeObtenido / e.PuntajeMaximo) * 100);
    }    /// <summary>
    /// Verifica si existe una evaluación para un período específico y cédula
    /// </summary>
    public async Task<bool> ExisteEvaluacionParaPeriodoAsync(string cedula, string periodoAcademico)
    {
        return await _dbContext.DAC
            .AnyAsync(e => e.Cedula == cedula && e.PeriodoAcademico == periodoAcademico && !e.IsDeleted);
    }

    /// <summary>
    /// Obtiene el archivo PDF de respaldo por ID
    /// </summary>
    public async Task<byte[]?> GetArchivoPdfByIdAsync(int id)
    {
        var evaluacion = await _dbContext.DAC
            .Where(e => e.Id == id && !e.IsDeleted)
            .Select(e => new { e.ArchivoRespaldo })
            .FirstOrDefaultAsync();

        return evaluacion?.ArchivoRespaldo;
    }    /// <summary>
    /// Obtiene estadísticas generales de evaluaciones
    /// </summary>
    public async Task<object> GetEstadisticasGeneralesAsync()
    {
        var evaluaciones = await _dbContext.DAC
            .Where(e => !e.IsDeleted)
            .ToListAsync();

        var totalEvaluaciones = evaluaciones.Count;
        var evaluacionesAprobadas = evaluaciones.Count(e => (e.PuntajeObtenido / e.PuntajeMaximo) * 100 >= 75);
        var docentesUnicos = evaluaciones.Select(e => e.Cedula).Distinct().Count();
        var promedioGeneral = evaluaciones.Any() ? evaluaciones.Average(e => (e.PuntajeObtenido / e.PuntajeMaximo) * 100) : 0;

        return new
        {
            TotalEvaluaciones = totalEvaluaciones,
            EvaluacionesAprobadas = evaluacionesAprobadas,
            PorcentajeAprobacion = totalEvaluaciones > 0 ? (decimal)evaluacionesAprobadas / totalEvaluaciones * 100 : 0,
            DocentesEvaluados = docentesUnicos,
            PromedioGeneral = Math.Round(promedioGeneral, 2)
        };
    }

    /// <summary>
    /// Método de prueba para verificar datos en tabla DAC
    /// </summary>
    public async Task<object> TestDacConnectionAsync()
    {
        try
        {
            var totalRegistros = await _dbContext.DAC.CountAsync();
            var registrosActivos = await _dbContext.DAC.CountAsync(e => !e.IsDeleted);
            var cedulasUnicas = await _dbContext.DAC
                .Where(e => !e.IsDeleted)
                .Select(e => e.Cedula)
                .Distinct()
                .CountAsync();

            var ultimosRegistros = await _dbContext.DAC
                .Where(e => !e.IsDeleted)
                .OrderByDescending(e => e.CreatedAt)
                .Take(5)
                .Select(e => new { e.Cedula, e.Anio, e.Semestre, e.PuntajeObtenido, e.PuntajeMaximo })
                .ToListAsync();

            return new
            {
                TotalRegistros = totalRegistros,
                RegistrosActivos = registrosActivos,
                CedulasUnicas = cedulasUnicas,
                UltimosRegistros = ultimosRegistros,
                Timestamp = DateTime.Now
            };
        }
        catch (Exception ex)
        {
            return new
            {
                Error = true,
                Message = ex.Message,
                InnerException = ex.InnerException?.Message,
                Timestamp = DateTime.Now
            };
        }
    }
}
