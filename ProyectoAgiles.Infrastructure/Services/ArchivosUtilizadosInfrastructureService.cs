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
            Console.WriteLine($"[ARCHIVOS] Iniciando registro inteligente de archivos para solicitud {solicitudEscalafonId}");
            
            // Verificar si ya existen archivos registrados para esta solicitud
            var archivosExistentes = await _context.ArchivosUtilizadosEscalafon
                .Where(a => a.SolicitudEscalafonId == solicitudEscalafonId)
                .CountAsync();
            
            if (archivosExistentes > 0)
            {
                Console.WriteLine($"[ARCHIVOS] Ya existen {archivosExistentes} archivos registrados para la solicitud {solicitudEscalafonId}. Saltando registro.");
                return;
            }
            
            // Determinar requisitos mínimos según el nivel de destino
            var requisitos = DeterminarRequisitos(nivelOrigen, nivelDestino);
            
            var archivosUtilizados = new List<ArchivosUtilizadosEscalafon>();

            // 1. Seleccionar investigaciones necesarias (ordenadas por fecha más antigua primero)
            if (requisitos.InvestigacionesMinimas > 0)
            {
                var investigaciones = await _context.Investigaciones
                    .Where(i => i.Cedula == docenteCedula)
                    .OrderBy(i => i.FechaPublicacion) // Más antiguas primero
                    .Take(requisitos.InvestigacionesMinimas)
                    .ToListAsync();

                Console.WriteLine($"[ARCHIVOS] Seleccionadas {investigaciones.Count} investigaciones de {requisitos.InvestigacionesMinimas} requeridas");

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
            }

            // 2. Seleccionar evaluaciones necesarias (ordenadas por fecha más antigua primero)
            if (requisitos.EvaluacionesMinimas > 0)
            {
                var evaluaciones = await _context.DAC
                    .Where(e => e.Cedula == docenteCedula)
                    .OrderBy(e => e.FechaEvaluacion) // Más antiguas primero
                    .Take(requisitos.EvaluacionesMinimas)
                    .ToListAsync();

                Console.WriteLine($"[ARCHIVOS] Seleccionadas {evaluaciones.Count} evaluaciones de {requisitos.EvaluacionesMinimas} requeridas");

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
                        Descripcion = $"{evaluacion.PeriodoAcademico} - {evaluacion.PorcentajeObtenido}%",
                        EstadoAscenso = "Aprobado"
                    });
                }
            }

            // 3. Seleccionar capacitaciones necesarias hasta cumplir las horas mínimas (ordenadas por fecha más antigua primero)
            if (requisitos.HorasCapacitacionMinimas > 0)
            {
                var capacitaciones = await _context.DITIC
                    .Where(c => c.Cedula == docenteCedula)
                    .OrderBy(c => c.FechaInicio) // Más antiguas primero
                    .ToListAsync();

                Console.WriteLine($"[ARCHIVOS] Evaluando capacitaciones para cumplir {requisitos.HorasCapacitacionMinimas} horas mínimas");

                int horasAcumuladas = 0;
                foreach (var capacitacion in capacitaciones)
                {
                    if (horasAcumuladas >= requisitos.HorasCapacitacionMinimas) break;

                    var horasCapacitacion = capacitacion.HorasAcademicas > 0 ? capacitacion.HorasAcademicas : 20; // Default 20 horas si no está especificado
                    horasAcumuladas += horasCapacitacion;

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

                Console.WriteLine($"[ARCHIVOS] Acumuladas {horasAcumuladas} horas con {archivosUtilizados.Count(a => a.TipoRecurso == "Capacitacion")} capacitaciones");
            }

            _context.ArchivosUtilizadosEscalafon.AddRange(archivosUtilizados);
            await _context.SaveChangesAsync();

            Console.WriteLine($"[ARCHIVOS] Registrados {archivosUtilizados.Count} documentos total para la solicitud {solicitudEscalafonId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ARCHIVOS] Error: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Determina los requisitos mínimos según el nivel de escalafón
    /// </summary>
    private RequisitosPorNivel DeterminarRequisitos(string nivelOrigen, string nivelDestino)
    {
        Console.WriteLine($"[ARCHIVOS] Determinando requisitos - Origen: '{nivelOrigen}', Destino: '{nivelDestino}'");
        
        // Definir requisitos según el nivel de destino
        // Estos valores podrían venir de una tabla de configuración en una implementación más avanzada
        var requisitos = nivelDestino.ToLower() switch
        {
            var nivel when nivel.Contains("auxiliar 2") => new RequisitosPorNivel 
            { 
                InvestigacionesMinimas = 2, 
                EvaluacionesMinimas = 3, 
                HorasCapacitacionMinimas = 80 
            },
            var nivel when nivel.Contains("auxiliar 3") => new RequisitosPorNivel 
            { 
                InvestigacionesMinimas = 3, 
                EvaluacionesMinimas = 4, 
                HorasCapacitacionMinimas = 100 
            },
            var nivel when nivel.Contains("agregado") => new RequisitosPorNivel 
            { 
                InvestigacionesMinimas = 4, 
                EvaluacionesMinimas = 4, 
                HorasCapacitacionMinimas = 120 
            },
            var nivel when nivel.Contains("principal") => new RequisitosPorNivel 
            { 
                InvestigacionesMinimas = 6, 
                EvaluacionesMinimas = 4, 
                HorasCapacitacionMinimas = 150 
            },
            _ => new RequisitosPorNivel 
            { 
                InvestigacionesMinimas = 2, 
                EvaluacionesMinimas = 3, 
                HorasCapacitacionMinimas = 80 
            }
        };

        Console.WriteLine($"[ARCHIVOS] Requisitos determinados - Investigaciones: {requisitos.InvestigacionesMinimas}, Evaluaciones: {requisitos.EvaluacionesMinimas}, Horas: {requisitos.HorasCapacitacionMinimas}");
        return requisitos;
    }

    /// <summary>
    /// Clase auxiliar para definir requisitos por nivel
    /// </summary>
    private class RequisitosPorNivel
    {
        public int InvestigacionesMinimas { get; set; }
        public int EvaluacionesMinimas { get; set; }
        public int HorasCapacitacionMinimas { get; set; }
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
