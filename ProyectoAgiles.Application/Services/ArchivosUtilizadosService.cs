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

    public ArchivosUtilizadosService(IArchivosUtilizadosRepository repository)
    {
        _repository = repository;
    }

    public async Task RegistrarArchivosUtilizados(int solicitudEscalafonId, string docenteCedula, string nivelOrigen, string nivelDestino)
    {
        try
        {
            // Necesitamos obtener los documentos reales que tiene el docente y registrarlos
            // TODO: Implementar inyección de dependencias para obtener documentos específicos
            
            Console.WriteLine($"[ARCHIVOS] Registrando archivos utilizados para solicitud {solicitudEscalafonId}, docente {docenteCedula}");
            
            // Por ahora, registramos que se utilizaron documentos específicos
            // Esto debe ser reemplazado por la lógica real para obtener investigaciones, evaluaciones y capacitaciones
            
            // Ejemplo: Si el docente tiene investigaciones, registrarlas
            await RegistrarDocumentosEspecificos(solicitudEscalafonId, docenteCedula, nivelOrigen, nivelDestino);
            
            Console.WriteLine($"[ARCHIVOS] Completado registro de archivos utilizados para solicitud {solicitudEscalafonId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ARCHIVOS] Error al registrar archivos utilizados: {ex.Message}");
            throw new InvalidOperationException($"Error al registrar archivos utilizados: {ex.Message}", ex);
        }
    }

    private async Task RegistrarDocumentosEspecificos(int solicitudEscalafonId, string docenteCedula, string nivelOrigen, string nivelDestino)
    {
        Console.WriteLine($"[ARCHIVOS] Iniciando registro de documentos específicos para solicitud {solicitudEscalafonId}");
        
        // Registrar un documento de cada tipo por ahora
        // TODO: Reemplazar por consulta real a las tablas de investigaciones, evaluaciones y capacitaciones
        
        var investigacion = new ArchivosUtilizadosEscalafon
        {
            SolicitudEscalafonId = solicitudEscalafonId,
            TipoRecurso = "Investigacion",
            RecursoId = GenerarIdTemporalBasadoEnSolicitud(solicitudEscalafonId, 1),
            DocenteCedula = docenteCedula,
            NivelOrigen = nivelOrigen,
            NivelDestino = nivelDestino,
            FechaUtilizacion = DateTime.Now,
            Descripcion = $"Artículo científico utilizado en promoción de {nivelOrigen} a {nivelDestino}",
            EstadoAscenso = "Aprobado"
        };
        
        Console.WriteLine($"[ARCHIVOS] Registrando investigación: ID={investigacion.RecursoId}, Descripción={investigacion.Descripcion}");
        await _repository.AddAsync(investigacion);

        var evaluacion = new ArchivosUtilizadosEscalafon
        {
            SolicitudEscalafonId = solicitudEscalafonId,
            TipoRecurso = "EvaluacionDesempeno", 
            RecursoId = GenerarIdTemporalBasadoEnSolicitud(solicitudEscalafonId, 2),
            DocenteCedula = docenteCedula,
            NivelOrigen = nivelOrigen,
            NivelDestino = nivelDestino,
            FechaUtilizacion = DateTime.Now,
            Descripcion = $"Evaluación DAC período 2024-2025 - Puntaje: 85%",
            EstadoAscenso = "Aprobado"
        };
        
        Console.WriteLine($"[ARCHIVOS] Registrando evaluación: ID={evaluacion.RecursoId}, Descripción={evaluacion.Descripcion}");
        await _repository.AddAsync(evaluacion);

        var capacitacion = new ArchivosUtilizadosEscalafon
        {
            SolicitudEscalafonId = solicitudEscalafonId,
            TipoRecurso = "Capacitacion",
            RecursoId = GenerarIdTemporalBasadoEnSolicitud(solicitudEscalafonId, 3),
            DocenteCedula = docenteCedula,
            NivelOrigen = nivelOrigen,
            NivelDestino = nivelDestino,
            FechaUtilizacion = DateTime.Now,
            Descripcion = $"Capacitación DITIC - Tecnologías Educativas (40 horas)",
            EstadoAscenso = "Aprobado"
        };
        
        Console.WriteLine($"[ARCHIVOS] Registrando capacitación: ID={capacitacion.RecursoId}, Descripción={capacitacion.Descripcion}");
        await _repository.AddAsync(capacitacion);
        
        Console.WriteLine($"[ARCHIVOS] Completado registro de 3 documentos para solicitud {solicitudEscalafonId}");
    }

    private int GenerarIdTemporalBasadoEnSolicitud(int solicitudId, int tipoDocumento)
    {
        // Generar IDs únicos basados en la solicitud para evitar conflictos
        return (solicitudId * 1000) + tipoDocumento;
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
