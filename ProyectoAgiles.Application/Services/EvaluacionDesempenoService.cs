using AutoMapper;
using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Application.Interfaces;
using ProyectoAgiles.Domain.Entities;
using ProyectoAgiles.Domain.Interfaces;

namespace ProyectoAgiles.Application.Services;

/// <summary>
/// Servicio para la gestión de evaluaciones de desempeño
/// </summary>
public class EvaluacionDesempenoService : IEvaluacionDesempenoService
{
    private readonly IEvaluacionDesempenoRepository _repository;
    private readonly IMapper _mapper;

    public EvaluacionDesempenoService(
        IEvaluacionDesempenoRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    /// <summary>
    /// Obtiene todas las evaluaciones
    /// </summary>
    public async Task<IEnumerable<EvaluacionDesempenoDto>> GetAllAsync()
    {
        var evaluaciones = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<EvaluacionDesempenoDto>>(evaluaciones);
    }

    /// <summary>
    /// Obtiene una evaluación por ID
    /// </summary>
    public async Task<EvaluacionDesempenoDto?> GetByIdAsync(int id)
    {
        var evaluacion = await _repository.GetByIdAsync(id);
        return evaluacion != null ? _mapper.Map<EvaluacionDesempenoDto>(evaluacion) : null;
    }

    /// <summary>
    /// Obtiene evaluaciones por cédula del docente
    /// </summary>
    public async Task<IEnumerable<EvaluacionDesempenoDto>> GetByCedulaAsync(string cedula)
    {
        var evaluaciones = await _repository.GetByCedulaAsync(cedula);
        return _mapper.Map<IEnumerable<EvaluacionDesempenoDto>>(evaluaciones);
    }

    /// <summary>
    /// Obtiene las últimas 4 evaluaciones de un docente
    /// </summary>
    public async Task<IEnumerable<EvaluacionDesempenoDto>> GetUltimasCuatroEvaluacionesByCedulaAsync(string cedula)
    {
        var evaluaciones = await _repository.GetUltimasCuatroEvaluacionesByCedulaAsync(cedula);
        return _mapper.Map<IEnumerable<EvaluacionDesempenoDto>>(evaluaciones);
    }

    /// <summary>
    /// Obtiene evaluaciones por período académico
    /// </summary>
    public async Task<IEnumerable<EvaluacionDesempenoDto>> GetByPeriodoAcademicoAsync(string periodoAcademico)
    {
        var evaluaciones = await _repository.GetByPeriodoAcademicoAsync(periodoAcademico);
        return _mapper.Map<IEnumerable<EvaluacionDesempenoDto>>(evaluaciones);
    }

    /// <summary>
    /// Obtiene evaluaciones por año
    /// </summary>
    public async Task<IEnumerable<EvaluacionDesempenoDto>> GetByAnioAsync(int anio)
    {
        var evaluaciones = await _repository.GetByAnioAsync(anio);
        return _mapper.Map<IEnumerable<EvaluacionDesempenoDto>>(evaluaciones);
    }

    /// <summary>
    /// Obtiene evaluaciones por año y semestre
    /// </summary>
    public async Task<IEnumerable<EvaluacionDesempenoDto>> GetByAnioAndSemestreAsync(int anio, int semestre)
    {
        var evaluaciones = await _repository.GetByAnioAndSemestreAsync(anio, semestre);
        return _mapper.Map<IEnumerable<EvaluacionDesempenoDto>>(evaluaciones);
    }

    /// <summary>
    /// Crea una nueva evaluación
    /// </summary>
    public async Task<EvaluacionDesempenoDto> CreateAsync(CreateEvaluacionDesempenoDto createDto)
    {
        // Verificar si ya existe una evaluación para este período y cédula
        var existe = await _repository.ExisteEvaluacionParaPeriodoAsync(createDto.Cedula, createDto.PeriodoAcademico);
        if (existe)
        {
            throw new InvalidOperationException($"Ya existe una evaluación para la cédula {createDto.Cedula} en el período {createDto.PeriodoAcademico}");
        }

        var evaluacion = _mapper.Map<EvaluacionDesempeno>(createDto);
        evaluacion.CreatedAt = DateTime.UtcNow;

        var created = await _repository.AddAsync(evaluacion);
        return _mapper.Map<EvaluacionDesempenoDto>(created);
    }

    /// <summary>
    /// Crea una nueva evaluación con archivo PDF
    /// </summary>
    public async Task<EvaluacionDesempenoDto> CreateWithPdfAsync(CreateEvaluacionWithPdfDto createDto)
    {
        // Verificar si ya existe una evaluación para este período y cédula
        var existe = await _repository.ExisteEvaluacionParaPeriodoAsync(createDto.Cedula, createDto.PeriodoAcademico);
        if (existe)
        {
            throw new InvalidOperationException($"Ya existe una evaluación para la cédula {createDto.Cedula} en el período {createDto.PeriodoAcademico}");
        }

        var evaluacion = _mapper.Map<EvaluacionDesempeno>(createDto);
        evaluacion.CreatedAt = DateTime.UtcNow;

        // Procesar el archivo PDF
        if (createDto.ArchivoPdf != null && createDto.ArchivoPdf.Length > 0)
        {
            using var memoryStream = new MemoryStream();
            await createDto.ArchivoPdf.CopyToAsync(memoryStream);
            evaluacion.ArchivoRespaldo = memoryStream.ToArray();
            evaluacion.NombreArchivoRespaldo = createDto.ArchivoPdf.FileName;
        }

        var created = await _repository.AddAsync(evaluacion);
        return _mapper.Map<EvaluacionDesempenoDto>(created);
    }

    /// <summary>
    /// Actualiza una evaluación existente
    /// </summary>
    public async Task<EvaluacionDesempenoDto> UpdateAsync(UpdateEvaluacionDesempenoDto updateDto)
    {
        var existingEvaluacion = await _repository.GetByIdAsync(updateDto.Id);
        if (existingEvaluacion == null)
        {
            throw new ArgumentException($"Evaluación con ID {updateDto.Id} no encontrada");
        }

        // Verificar si el cambio de período causa duplicados
        if (existingEvaluacion.PeriodoAcademico != updateDto.PeriodoAcademico ||
            existingEvaluacion.Cedula != updateDto.Cedula)
        {
            var existe = await _repository.ExisteEvaluacionParaPeriodoAsync(updateDto.Cedula, updateDto.PeriodoAcademico);
            if (existe)
            {
                throw new InvalidOperationException($"Ya existe una evaluación para la cédula {updateDto.Cedula} en el período {updateDto.PeriodoAcademico}");
            }
        }

        _mapper.Map(updateDto, existingEvaluacion);
        existingEvaluacion.UpdatedAt = DateTime.UtcNow;

        var updated = await _repository.UpdateAsync(existingEvaluacion);
        return _mapper.Map<EvaluacionDesempenoDto>(updated);
    }

    /// <summary>
    /// Elimina una evaluación (soft delete)
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        var evaluacion = await _repository.GetByIdAsync(id);
        if (evaluacion == null)
            return false;

        evaluacion.IsDeleted = true;
        evaluacion.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(evaluacion);
        return true;
    }

    /// <summary>
    /// Verifica si existe una evaluación
    /// </summary>
    public async Task<bool> ExistsAsync(int id)
    {
        var evaluacion = await _repository.GetByIdAsync(id);
        return evaluacion != null && !evaluacion.IsDeleted;
    }

    /// <summary>
    /// Obtiene el resumen de evaluaciones de un docente
    /// </summary>
    public async Task<ResumenEvaluacionesDto> GetResumenEvaluacionesByCedulaAsync(string cedula)
    {
        var todasLasEvaluaciones = await _repository.GetByCedulaAsync(cedula);
        var ultimasCuatro = await _repository.GetUltimasCuatroEvaluacionesByCedulaAsync(cedula);

        var totalEvaluaciones = todasLasEvaluaciones.Count();
        var evaluacionesAprobadas = todasLasEvaluaciones.Count(e => (e.PuntajeObtenido / e.PuntajeMaximo) * 100 >= 75);
        var porcentajeAprobacion = totalEvaluaciones > 0 ? (decimal)evaluacionesAprobadas / totalEvaluaciones * 100 : 0;
        var promedioGeneral = todasLasEvaluaciones.Any() ? 
            todasLasEvaluaciones.Average(e => (e.PuntajeObtenido / e.PuntajeMaximo) * 100) : 0;

        var cumpleRequisito = ultimasCuatro.Count() >= 4 && 
                             ultimasCuatro.All(e => (e.PuntajeObtenido / e.PuntajeMaximo) * 100 >= 75);

        return new ResumenEvaluacionesDto
        {
            Cedula = cedula,
            TotalEvaluaciones = totalEvaluaciones,
            EvaluacionesAprobadas = evaluacionesAprobadas,
            PorcentajeAprobacion = Math.Round(porcentajeAprobacion, 2),
            PromedioGeneral = Math.Round(promedioGeneral, 2),
            CumpleRequisito75Porciento = cumpleRequisito,
            UltimasCuatroEvaluaciones = _mapper.Map<List<EvaluacionDesempenoDto>>(ultimasCuatro)
        };
    }

    /// <summary>
    /// Verifica si un docente cumple con el requisito del 75%
    /// </summary>
    public async Task<VerificacionRequisito75Dto> VerificarRequisito75PorCientoAsync(string cedula)
    {
        var ultimasCuatro = await _repository.GetUltimasCuatroEvaluacionesByCedulaAsync(cedula);
        var evaluacionesConsideradas = _mapper.Map<List<EvaluacionDesempenoDto>>(ultimasCuatro);

        var evaluacionesAnalizadas = ultimasCuatro.Count();
        var evaluacionesQueAlcanzan75 = ultimasCuatro.Count(e => (e.PuntajeObtenido / e.PuntajeMaximo) * 100 >= 75);
        var promedioUltimasCuatro = ultimasCuatro.Any() ? 
            ultimasCuatro.Average(e => (e.PuntajeObtenido / e.PuntajeMaximo) * 100) : 0;

        bool cumpleRequisito = evaluacionesAnalizadas >= 4 && evaluacionesQueAlcanzan75 == evaluacionesAnalizadas;

        string mensaje;
        if (evaluacionesAnalizadas < 4)
        {
            mensaje = $"El docente solo tiene {evaluacionesAnalizadas} evaluaciones. Se requieren mínimo 4 evaluaciones.";
        }
        else if (cumpleRequisito)
        {
            mensaje = "El docente cumple con el requisito del 75% en las últimas 4 evaluaciones.";
        }
        else
        {
            mensaje = $"El docente NO cumple con el requisito. Solo {evaluacionesQueAlcanzan75} de {evaluacionesAnalizadas} evaluaciones alcanzan el 75%.";
        }

        return new VerificacionRequisito75Dto
        {
            Cedula = cedula,
            CumpleRequisito = cumpleRequisito,
            EvaluacionesAnalizadas = evaluacionesAnalizadas,
            EvaluacionesQueAlcanzan75 = evaluacionesQueAlcanzan75,
            PorcentajePromedioUltimasCuatro = Math.Round(promedioUltimasCuatro, 2),
            Mensaje = mensaje,
            EvaluacionesConsideradas = evaluacionesConsideradas
        };
    }

    /// <summary>
    /// Obtiene evaluaciones que alcanzan el 75%
    /// </summary>
    public async Task<IEnumerable<EvaluacionDesempenoDto>> GetEvaluacionesQueAlcanzan75PorCientoAsync()
    {
        var evaluaciones = await _repository.GetEvaluacionesQueAlcanzan75PorCientoAsync();
        return _mapper.Map<IEnumerable<EvaluacionDesempenoDto>>(evaluaciones);
    }

    /// <summary>
    /// Obtiene evaluaciones que alcanzan el 75% por cédula
    /// </summary>
    public async Task<IEnumerable<EvaluacionDesempenoDto>> GetEvaluacionesQueAlcanzan75PorCientoByCedulaAsync(string cedula)
    {
        var evaluaciones = await _repository.GetEvaluacionesQueAlcanzan75PorCientoByCedulaAsync(cedula);
        return _mapper.Map<IEnumerable<EvaluacionDesempenoDto>>(evaluaciones);
    }

    /// <summary>
    /// Obtiene el archivo PDF por ID
    /// </summary>
    public async Task<byte[]?> GetPdfByIdAsync(int id)
    {
        return await _repository.GetArchivoPdfByIdAsync(id);
    }

    /// <summary>
    /// Obtiene estadísticas generales
    /// </summary>
    public async Task<object> GetEstadisticasGeneralesAsync()
    {
        return await _repository.GetEstadisticasGeneralesAsync();
    }

    /// <summary>
    /// Verifica si existe evaluación para un período específico
    /// </summary>
    public async Task<bool> ExisteEvaluacionParaPeriodoAsync(string cedula, string periodoAcademico)
    {
        return await _repository.ExisteEvaluacionParaPeriodoAsync(cedula, periodoAcademico);
    }
}
