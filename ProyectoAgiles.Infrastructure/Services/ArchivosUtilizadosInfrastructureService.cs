using Microsoft.EntityFrameworkCore;
using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Application.Interfaces;
using ProyectoAgiles.Domain.Entities;
using ProyectoAgiles.Infrastructure.Data;

namespace ProyectoAgiles.Infrastructure.Services;

/// <summary>
/// Servicio de infraestructura para gestionar archivos utilizados en escalafones
/// Tiene acceso directo al DbContext para operaciones complejas
/// </summary>
public class ArchivosUtilizadosInfrastructureService : IArchivosUtilizadosService
{
    private readonly ApplicationDbContext _context;

    public ArchivosUtilizadosInfrastructureService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task RegistrarArchivosUtilizados(int solicitudEscalafonId, string docenteCedula, string nivelOrigen, string nivelDestino)
    {
        try
        {
            // Obtener investigaciones del docente
            var investigaciones = await _context.Investigaciones
                .Where(i => i.Cedula == docenteCedula)
                .Select(i => new { i.Id, i.Titulo })
                .ToListAsync();

            // Obtener evaluaciones del docente (últimas 4)
            var evaluaciones = await _context.DAC
                .Where(e => e.Cedula == docenteCedula)
                .OrderByDescending(e => e.FechaEvaluacion)
                .Take(4)
                .Select(e => new { e.Id, Titulo = $"{e.PeriodoAcademico} - {e.PorcentajeObtenido}%" })
                .ToListAsync();

            // Obtener capacitaciones del docente
            var capacitaciones = await _context.DITIC
                .Where(c => c.Cedula == docenteCedula)
                .Select(c => new { c.Id, c.NombreCapacitacion })
                .ToListAsync();

            var archivosUtilizados = new List<ArchivosUtilizadosEscalafon>();

            // Registrar investigaciones utilizadas
            foreach (var investigacion in investigaciones)
            {
                archivosUtilizados.Add(new ArchivosUtilizadosEscalafon
                {
                    SolicitudEscalafonId = solicitudEscalafonId,
                    TipoRecurso = "Investigacion",
                    RecursoId = investigacion.Id,
                    DocenteCedula = docenteCedula,
                    NivelOrigen = nivelOrigen,
                    NivelDestino = nivelDestino,
                    FechaUtilizacion = DateTime.UtcNow,
                    Descripcion = investigacion.Titulo,
                    EstadoAscenso = "Aprobado"
                });
            }

            // Registrar evaluaciones utilizadas
            foreach (var evaluacion in evaluaciones)
            {
                archivosUtilizados.Add(new ArchivosUtilizadosEscalafon
                {
                    SolicitudEscalafonId = solicitudEscalafonId,
                    TipoRecurso = "EvaluacionDesempeno",
                    RecursoId = evaluacion.Id,
                    DocenteCedula = docenteCedula,
                    NivelOrigen = nivelOrigen,
                    NivelDestino = nivelDestino,
                    FechaUtilizacion = DateTime.UtcNow,
                    Descripcion = evaluacion.Titulo,
                    EstadoAscenso = "Aprobado"
                });
            }

            // Registrar capacitaciones utilizadas
            foreach (var capacitacion in capacitaciones)
            {
                archivosUtilizados.Add(new ArchivosUtilizadosEscalafon
                {
                    SolicitudEscalafonId = solicitudEscalafonId,
                    TipoRecurso = "Capacitacion",
                    RecursoId = capacitacion.Id,
                    DocenteCedula = docenteCedula,
                    NivelOrigen = nivelOrigen,
                    NivelDestino = nivelDestino,
                    FechaUtilizacion = DateTime.UtcNow,
                    Descripcion = capacitacion.NombreCapacitacion,
                    EstadoAscenso = "Aprobado"
                });
            }

            _context.ArchivosUtilizadosEscalafon.AddRange(archivosUtilizados);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al registrar archivos utilizados: {ex.Message}", ex);
        }
    }

    public async Task<List<int>> ObtenerInvestigacionesUtilizadas(string docenteCedula)
    {
        return await _context.ArchivosUtilizadosEscalafon
            .Where(a => a.DocenteCedula == docenteCedula && 
                       a.TipoRecurso == "Investigacion" && 
                       a.EstadoAscenso == "Aprobado")
            .Select(a => a.RecursoId)
            .Distinct()
            .ToListAsync();
    }

    public async Task<List<int>> ObtenerEvaluacionesUtilizadas(string docenteCedula)
    {
        return await _context.ArchivosUtilizadosEscalafon
            .Where(a => a.DocenteCedula == docenteCedula && 
                       a.TipoRecurso == "EvaluacionDesempeno" && 
                       a.EstadoAscenso == "Aprobado")
            .Select(a => a.RecursoId)
            .Distinct()
            .ToListAsync();
    }

    public async Task<List<int>> ObtenerCapacitacionesUtilizadas(string docenteCedula)
    {
        return await _context.ArchivosUtilizadosEscalafon
            .Where(a => a.DocenteCedula == docenteCedula && 
                       a.TipoRecurso == "Capacitacion" && 
                       a.EstadoAscenso == "Aprobado")
            .Select(a => a.RecursoId)
            .Distinct()
            .ToListAsync();
    }

    public async Task<List<ArchivosUtilizadosDto>> ObtenerHistorialArchivos(string docenteCedula)
    {
        return await _context.ArchivosUtilizadosEscalafon
            .Where(a => a.DocenteCedula == docenteCedula)
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
            .ToListAsync();
    }

    public async Task<bool> ArchivoYaUtilizado(string docenteCedula, string tipoRecurso, int recursoId)
    {
        return await _context.ArchivosUtilizadosEscalafon
            .AnyAsync(a => a.DocenteCedula == docenteCedula && 
                          a.TipoRecurso == tipoRecurso && 
                          a.RecursoId == recursoId && 
                          a.EstadoAscenso == "Aprobado");
    }

    public async Task<Dictionary<string, int>> ObtenerEstadisticasArchivosUtilizados(string docenteCedula)
    {
        var estadisticas = await _context.ArchivosUtilizadosEscalafon
            .Where(a => a.DocenteCedula == docenteCedula && a.EstadoAscenso == "Aprobado")
            .GroupBy(a => a.TipoRecurso)
            .Select(g => new { TipoRecurso = g.Key, Cantidad = g.Count() })
            .ToDictionaryAsync(x => x.TipoRecurso, x => x.Cantidad);

        return estadisticas;
    }

    public async Task<List<ArchivosUtilizadosDto>> ObtenerArchivosPorSolicitud(int solicitudEscalafonId)
    {
        var archivos = await _context.ArchivosUtilizadosEscalafon
            .Where(a => a.SolicitudEscalafonId == solicitudEscalafonId)
            .OrderBy(a => a.TipoRecurso)
            .ToListAsync();

        return archivos.Select(a => new ArchivosUtilizadosDto
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
            TituloRecurso = a.Descripcion ?? ObtenerTituloGenerico(a.TipoRecurso),
            DetallesRecurso = a.Descripcion ?? "Sin detalles específicos"
        }).ToList();
    }

    private static string ObtenerTituloGenerico(string tipoRecurso)
    {
        return tipoRecurso switch
        {
            "Investigacion" => "Publicación científica",
            "EvaluacionDesempeno" => "Evaluación de desempeño",
            "Capacitacion" => "Capacitación profesional",
            _ => "Documento académico"
        };
    }
}
