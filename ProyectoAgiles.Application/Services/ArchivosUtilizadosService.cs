using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Application.Interfaces;
using ProyectoAgiles.Domain.Interfaces;
using ProyectoAgiles.Domain.Entities;

namespace ProyectoAgiles.Application.Services;

/// <summary>
/// Servicio para gestionar archivos utilizados en escalafones (capa de aplicación)
/// </summary>
public class ArchivosUtilizadosService : IArchivosUtilizadosService
{
    private readonly IArchivosUtilizadosRepository _repository;
    private readonly IInvestigacionService _investigacionService;
    private readonly IEvaluacionDesempenoService _evaluacionService;
    private readonly IDiticService _diticService;
    private readonly IRequisitosEscalafonService _requisitosService;

    public ArchivosUtilizadosService(
        IArchivosUtilizadosRepository repository,
        IInvestigacionService investigacionService,
        IEvaluacionDesempenoService evaluacionService,
        IDiticService diticService,
        IRequisitosEscalafonService requisitosService)
    {
        _repository = repository;
        _investigacionService = investigacionService;
        _evaluacionService = evaluacionService;
        _diticService = diticService;
        _requisitosService = requisitosService;
    }

    public async Task RegistrarArchivosUtilizados(int solicitudEscalafonId, string docenteCedula, string nivelOrigen, string nivelDestino)
    {
        try
        {
            Console.WriteLine($"[ARCHIVOS] Registrando archivos utilizados para solicitud {solicitudEscalafonId}, docente {docenteCedula}");
            Console.WriteLine($"[ARCHIVOS] Promoción: {nivelOrigen} → {nivelDestino}");
            
            // Obtener configuración de requisitos para este nivel
            var configuracion = _requisitosService.GetRequisitosParaNivel(nivelOrigen);
            if (configuracion == null)
            {
                Console.WriteLine($"[ARCHIVOS] No se encontró configuración de requisitos para nivel {nivelOrigen}");
                return;
            }
            
            // Obtener investigaciones disponibles y seleccionar las más antiguas
            var investigacionesDisponibles = (await _investigacionService.GetDisponiblesParaEscalafonAsync(docenteCedula)).ToList();
            var investigacionesSeleccionadas = investigacionesDisponibles
                .Where(inv => !string.IsNullOrEmpty(inv.Filiacion) && 
                             inv.Filiacion.ToUpper().Contains("UTA"))
                .OrderBy(inv => inv.FechaPublicacion)
                .Take(configuracion.ObrasRelevantesMinimoTotal)
                .ToList();
            
            Console.WriteLine($"[ARCHIVOS] Investigaciones disponibles: {investigacionesDisponibles.Count}, seleccionadas: {investigacionesSeleccionadas.Count}");
            
            // Registrar investigaciones utilizadas
            foreach (var investigacion in investigacionesSeleccionadas)
            {
                var archivoUtilizado = new ArchivosUtilizadosEscalafon
                {
                    SolicitudEscalafonId = solicitudEscalafonId,
                    TipoRecurso = "Investigacion",
                    RecursoId = investigacion.Id,
                    DocenteCedula = docenteCedula,
                    NivelOrigen = nivelOrigen,
                    NivelDestino = nivelDestino,
                    FechaUtilizacion = DateTime.Now,
                    Descripcion = $"Investigación: {investigacion.Titulo}",
                    EstadoAscenso = "Aprobado"
                };
                
                await _repository.AddAsync(archivoUtilizado);
                Console.WriteLine($"[ARCHIVOS] Registrada investigación ID {investigacion.Id}: {investigacion.Titulo}");
            }
            
            // Obtener evaluaciones disponibles y seleccionar las más antiguas con puntaje >= 75%
            var evaluacionesDisponibles = (await _evaluacionService.GetDisponiblesParaEscalafonAsync(docenteCedula)).ToList();
            var evaluacionesSeleccionadas = evaluacionesDisponibles
                .Where(eval => eval.PorcentajeObtenido >= 75)
                .OrderBy(eval => eval.Anio)
                .ThenBy(eval => eval.Semestre)
                .Take(configuracion.PeriodosEvaluacionRequeridos)
                .ToList();
            
            Console.WriteLine($"[ARCHIVOS] Evaluaciones disponibles: {evaluacionesDisponibles.Count}, seleccionadas: {evaluacionesSeleccionadas.Count}");
            
            // Registrar evaluaciones utilizadas
            foreach (var evaluacion in evaluacionesSeleccionadas)
            {
                var archivoUtilizado = new ArchivosUtilizadosEscalafon
                {
                    SolicitudEscalafonId = solicitudEscalafonId,
                    TipoRecurso = "EvaluacionDesempeno",
                    RecursoId = evaluacion.Id,
                    DocenteCedula = docenteCedula,
                    NivelOrigen = nivelOrigen,
                    NivelDestino = nivelDestino,
                    FechaUtilizacion = DateTime.Now,
                    Descripcion = $"Evaluación {evaluacion.PeriodoAcademico}: {evaluacion.PorcentajeObtenido}%",
                    EstadoAscenso = "Aprobado"
                };
                
                await _repository.AddAsync(archivoUtilizado);
                Console.WriteLine($"[ARCHIVOS] Registrada evaluación ID {evaluacion.Id}: {evaluacion.PeriodoAcademico}");
            }
            
            // Obtener capacitaciones disponibles y seleccionar hasta cumplir las horas requeridas
            var capacitacionesDisponibles = (await _diticService.GetDisponiblesParaEscalafonAsync(docenteCedula)).ToList();
            var capacitacionesSeleccionadas = new List<DiticDto>();
            int horasAcumuladas = 0;
            
            foreach (var capacitacion in capacitacionesDisponibles.OrderBy(c => c.FechaInicio))
            {
                if (horasAcumuladas >= configuracion.HorasCapacitacionRequeridas)
                    break;
                    
                capacitacionesSeleccionadas.Add(capacitacion);
                horasAcumuladas += capacitacion.HorasAcademicas;
            }
            
            Console.WriteLine($"[ARCHIVOS] Capacitaciones disponibles: {capacitacionesDisponibles.Count}, seleccionadas: {capacitacionesSeleccionadas.Count}");
            
            // Registrar capacitaciones utilizadas
            foreach (var capacitacion in capacitacionesSeleccionadas)
            {
                var archivoUtilizado = new ArchivosUtilizadosEscalafon
                {
                    SolicitudEscalafonId = solicitudEscalafonId,
                    TipoRecurso = "Capacitacion",
                    RecursoId = capacitacion.Id,
                    DocenteCedula = docenteCedula,
                    NivelOrigen = nivelOrigen,
                    NivelDestino = nivelDestino,
                    FechaUtilizacion = DateTime.Now,
                    Descripcion = $"Capacitación: {capacitacion.NombreCapacitacion} ({capacitacion.HorasAcademicas}h)",
                    EstadoAscenso = "Aprobado"
                };
                
                await _repository.AddAsync(archivoUtilizado);
                Console.WriteLine($"[ARCHIVOS] Registrada capacitación ID {capacitacion.Id}: {capacitacion.NombreCapacitacion}");
            }
            
            Console.WriteLine($"[ARCHIVOS] Completado registro de archivos utilizados para solicitud {solicitudEscalafonId}");
            Console.WriteLine($"[ARCHIVOS] Total registrado: {investigacionesSeleccionadas.Count} investigaciones, {evaluacionesSeleccionadas.Count} evaluaciones, {capacitacionesSeleccionadas.Count} capacitaciones");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ARCHIVOS] Error al registrar archivos utilizados: {ex.Message}");
            throw new InvalidOperationException($"Error al registrar archivos utilizados: {ex.Message}", ex);
        }
    }

    public async Task<List<int>> ObtenerInvestigacionesUtilizadas(string docenteCedula)
    {
        var archivos = await _repository.GetByDocenteCedulaAsync(docenteCedula);
        return archivos
            .Where(a => a.TipoRecurso == "Investigacion" && a.EstadoAscenso == "Aprobado")
            .Select(a => a.RecursoId)
            .Distinct()
            .ToList();
    }

    public async Task<List<int>> ObtenerEvaluacionesUtilizadas(string docenteCedula)
    {
        var archivos = await _repository.GetByDocenteCedulaAsync(docenteCedula);
        return archivos
            .Where(a => a.TipoRecurso == "EvaluacionDesempeno" && a.EstadoAscenso == "Aprobado")
            .Select(a => a.RecursoId)
            .Distinct()
            .ToList();
    }

    public async Task<List<int>> ObtenerCapacitacionesUtilizadas(string docenteCedula)
    {
        var archivos = await _repository.GetByDocenteCedulaAsync(docenteCedula);
        return archivos
            .Where(a => a.TipoRecurso == "Capacitacion" && a.EstadoAscenso == "Aprobado")
            .Select(a => a.RecursoId)
            .Distinct()
            .ToList();
    }

    public async Task<List<ArchivosUtilizadosDto>> ObtenerHistorialArchivos(string docenteCedula)
    {
        var archivos = await _repository.GetByDocenteCedulaAsync(docenteCedula);
        return archivos
            .OrderByDescending(a => a.FechaUtilizacion)
            .Select(a => new ArchivosUtilizadosDto
            {
                Id = a.Id,
                SolicitudEscalafonId = a.SolicitudEscalafonId,
                TipoRecurso = a.TipoRecurso,
                RecursoId = a.RecursoId,
                DocenteCedula = a.DocenteCedula,
                NivelOrigen = a.NivelOrigen,
                NivelDestino = a.NivelDestino,
                FechaUtilizacion = a.FechaUtilizacion,
                Descripcion = a.Descripcion,
                EstadoAscenso = a.EstadoAscenso,
                TituloRecurso = a.Descripcion,
                DetallesRecurso = $"{a.TipoRecurso} utilizado para ascender de {a.NivelOrigen} a {a.NivelDestino}"
            })
            .ToList();
    }

    public async Task<bool> ArchivoYaUtilizado(string docenteCedula, string tipoRecurso, int recursoId)
    {
        return await _repository.IsRecursoUtilizadoAsync(docenteCedula, tipoRecurso, recursoId);
    }

    public async Task<Dictionary<string, int>> ObtenerEstadisticasArchivosUtilizados(string docenteCedula)
    {
        var archivos = await _repository.GetByDocenteCedulaAsync(docenteCedula);
        return archivos
            .Where(a => a.EstadoAscenso == "Aprobado")
            .GroupBy(a => a.TipoRecurso)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public async Task<List<ArchivosUtilizadosDto>> ObtenerArchivosPorSolicitud(int solicitudEscalafonId)
    {
        try
        {
            Console.WriteLine($"[ARCHIVOS] Obteniendo archivos para solicitud {solicitudEscalafonId}");
            
            // Obtener archivos específicos de esta solicitud
            var archivos = await _repository.GetBySolicitudEscalafonIdAsync(solicitudEscalafonId);
            
            Console.WriteLine($"[ARCHIVOS] Archivos encontrados: {archivos.Count}");
            
            return archivos
                .OrderBy(a => a.TipoRecurso)
                .Select(a => new ArchivosUtilizadosDto
                {
                    Id = a.Id,
                    SolicitudEscalafonId = a.SolicitudEscalafonId,
                    TipoRecurso = a.TipoRecurso,
                    RecursoId = a.RecursoId,
                    DocenteCedula = a.DocenteCedula,
                    NivelOrigen = a.NivelOrigen,
                    NivelDestino = a.NivelDestino,
                    FechaUtilizacion = a.FechaUtilizacion,
                    Descripcion = a.Descripcion,
                    EstadoAscenso = a.EstadoAscenso,
                    TituloRecurso = ObtenerTituloEspecifico(a.TipoRecurso, a.Descripcion),
                    DetallesRecurso = a.Descripcion ?? "Sin detalles específicos"
                })
                .ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ARCHIVOS] Error obteniendo archivos por solicitud: {ex.Message}");
            return new List<ArchivosUtilizadosDto>();
        }
    }

    private string ObtenerTituloEspecifico(string tipoRecurso, string? descripcion)
    {
        if (string.IsNullOrEmpty(descripcion))
        {
            return tipoRecurso switch
            {
                "Investigacion" => "Publicación científica",
                "EvaluacionDesempeno" => "Evaluación de desempeño",
                "Capacitacion" => "Capacitación profesional",
                _ => "Documento académico"
            };
        }
        return descripcion;
    }
}
