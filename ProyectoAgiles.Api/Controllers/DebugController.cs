using Microsoft.AspNetCore.Mvc;
using ProyectoAgiles.Application.Interfaces;
using System.Text.Json;

namespace ProyectoAgiles.Api.Controllers;

/// <summary>
/// Controlador temporal para diagnosticar problemas con archivos utilizados
/// </summary>
[ApiController]
[Route("api/debug")]
public class DebugController : ControllerBase
{
    private readonly IArchivosUtilizadosService _archivosService;
    private readonly ISolicitudEscalafonService _solicitudService;

    public DebugController(
        IArchivosUtilizadosService archivosService,
        ISolicitudEscalafonService solicitudService)
    {
        _archivosService = archivosService;
        _solicitudService = solicitudService;
    }

    /// <summary>
    /// Obtiene todos los archivos utilizados para depuración
    /// </summary>
    [HttpGet("archivos-utilizados/{cedula}")]
    public async Task<IActionResult> GetArchivosUtilizados(string cedula)
    {
        try
        {
            var archivos = await _archivosService.ObtenerHistorialArchivos(cedula);
            return Ok(new
            {
                Cedula = cedula,
                TotalArchivos = archivos.Count,
                Archivos = archivos
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene archivos por solicitud específica
    /// </summary>
    [HttpGet("archivos-por-solicitud/{solicitudId}")]
    public async Task<IActionResult> GetArchivosPorSolicitud(int solicitudId)
    {
        try
        {
            var archivos = await _archivosService.ObtenerArchivosPorSolicitud(solicitudId);
            return Ok(new
            {
                SolicitudId = solicitudId,
                TotalArchivos = archivos.Count,
                Archivos = archivos
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Simula el registro de archivos para una solicitud
    /// </summary>
    [HttpPost("simular-registro/{solicitudId}")]
    public async Task<IActionResult> SimularRegistroArchivos(int solicitudId)
    {
        try
        {
            // Obtener la solicitud primero
            var solicitud = await _solicitudService.GetSolicitudByIdAsync(solicitudId);
            if (solicitud == null)
            {
                return NotFound($"Solicitud {solicitudId} no encontrada");
            }

            // Registrar archivos utilizados
            await _archivosService.RegistrarArchivosUtilizados(
                solicitudId,
                solicitud.DocenteCedula,
                solicitud.NivelActual,
                solicitud.NivelSolicitado
            );

            // Obtener los archivos registrados
            var archivos = await _archivosService.ObtenerArchivosPorSolicitud(solicitudId);

            return Ok(new
            {
                Mensaje = "Archivos registrados exitosamente",
                SolicitudId = solicitudId,
                TotalArchivos = archivos.Count,
                Archivos = archivos
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene el historial completo de una cédula con verificación de requisitos opcional
    /// </summary>
    [HttpGet("historial-completo/{cedula}")]
    public async Task<IActionResult> GetHistorialCompleto(string cedula, [FromQuery] bool incluirVerificacion = false)
    {
        try
        {
            var historial = await _solicitudService.GetHistorialEscalafonAsync(cedula);
            
            if (incluirVerificacion)
            {
                Console.WriteLine($"[DEBUG] Obteniendo verificación de requisitos para cédula: {cedula}");
                
                // Obtener la verificación de requisitos actual
                object verificacionRequisitos = null;
                
                try
                {
                    // Llamar a la API de verificación de requisitos
                    using var httpClient = new HttpClient();
                    httpClient.BaseAddress = new Uri(Request.Scheme + "://" + Request.Host);
                    
                    var url = $"/api/EvaluacionesDesempeno/estadisticas-docente/{cedula}";
                    Console.WriteLine($"[DEBUG] Llamando a URL: {url}");
                    
                    var response = await httpClient.GetAsync(url);
                    
                    Console.WriteLine($"[DEBUG] Response status: {response.StatusCode}");
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"[DEBUG] Response content length: {responseContent.Length}");
                        verificacionRequisitos = JsonSerializer.Deserialize<object>(responseContent);
                        Console.WriteLine($"[DEBUG] Verificación de requisitos obtenida exitosamente");
                    }
                    else
                    {
                        Console.WriteLine($"[DEBUG] Error en respuesta: {response.StatusCode}");
                        verificacionRequisitos = new
                        {
                            error = "No se pudo obtener la verificación de requisitos",
                            status = (int)response.StatusCode
                        };
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DEBUG] Exception en verificación: {ex.Message}");
                    verificacionRequisitos = new
                    {
                        error = $"Error al consultar API de verificación: {ex.Message}"
                    };
                }
                
                Console.WriteLine($"[DEBUG] Devolviendo respuesta con verificación");
                
                return Ok(new
                {
                    Cedula = cedula,
                    TotalRegistros = historial.Count(),
                    Historial = historial,
                    VerificacionRequisitosActual = verificacionRequisitos,
                    Metadata = new
                    {
                        IncluirVerificacionRequisitos = true,
                        ApiVerificacionUtilizada = "/api/EvaluacionesDesempeno/estadisticas-docente/{cedula}",
                        Descripcion = "Historial de escalafones con verificación de requisitos actual agregada"
                    }
                });
            }
            
            return Ok(new
            {
                Cedula = cedula,
                TotalRegistros = historial.Count(),
                Historial = historial,
                OpcionesDisponibles = new
                {
                    VerificacionRequisitos = $"/api/debug/historial-completo/{cedula}?incluirVerificacion=true",
                    HistorialConVerificacion = $"/api/debug/historial-con-verificacion/{cedula}",
                    DepuracionAvanzada = $"/api/debug/historial-debug/{cedula}"
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DEBUG] Exception general: {ex.Message}");
            return BadRequest(new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Lista todas las solicitudes de escalafón para depuración
    /// </summary>
    [HttpGet("solicitudes")]
    public async Task<IActionResult> GetTodasLasSolicitudes()
    {
        try
        {
            var solicitudes = await _solicitudService.GetAllSolicitudesAsync();
            return Ok(new
            {
                Total = solicitudes.Count(),
                Solicitudes = solicitudes.Select(s => new
                {
                    s.Id,
                    s.DocenteCedula,
                    s.NivelActual,
                    s.NivelSolicitado,
                    s.FechaSolicitud,
                    s.Status
                })
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Lista todos los docentes con investigaciones para depuración
    /// </summary>
    [HttpGet("docentes-con-investigaciones")]
    public async Task<IActionResult> GetDocentesConInvestigaciones()
    {
        try
        {
            // Este método necesitaríamos implementarlo en el servicio
            return Ok(new { Mensaje = "Endpoint en desarrollo" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Limpia archivos duplicados para una solicitud específica
    /// </summary>
    [HttpDelete("limpiar-archivos/{solicitudId}")]
    public async Task<IActionResult> LimpiarArchivos(int solicitudId)
    {
        try
        {
            // Este endpoint necesitaríamos implementar una funcionalidad de limpieza en el servicio
            // Por ahora solo retornamos información
            var archivos = await _archivosService.ObtenerArchivosPorSolicitud(solicitudId);
            return Ok(new
            {
                Mensaje = "Funcionalidad de limpieza en desarrollo",
                SolicitudId = solicitudId,
                ArchivosActuales = archivos.Count
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Elimina todos los archivos utilizados para una solicitud específica para poder volver a probar
    /// </summary>
    [HttpDelete("limpiar-archivos-solicitud/{solicitudId}")]
    public async Task<IActionResult> LimpiarArchivosSolicitud(int solicitudId)
    {
        try
        {
            // Implementar la limpieza directamente aquí por simplicidad
            return Ok(new
            {
                Mensaje = "Para limpiar archivos, use SQL: DELETE FROM ArchivosUtilizadosEscalafon WHERE SolicitudEscalafonId = " + solicitudId,
                SolicitudId = solicitudId
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene archivos utilizados filtrados estrictamente por cédula
    /// </summary>
    [HttpGet("archivos-cedula-estricto/{cedula}")]
    public async Task<IActionResult> GetArchivosEstrictoPorCedula(string cedula)
    {
        try
        {
            var archivos = await _archivosService.ObtenerHistorialArchivos(cedula);
            
            // Filtrar adicionalmente por cédula para asegurar que solo vengan de esa cédula
            var archivosFiltrados = archivos.Where(a => a.DocenteCedula == cedula).ToList();
            
            return Ok(new
            {
                CedulaSolicitada = cedula,
                TotalArchivos = archivosFiltrados.Count,
                Archivos = archivosFiltrados.Select(a => new 
                {
                    a.Id,
                    a.SolicitudEscalafonId,
                    a.TipoRecurso,
                    a.RecursoId,
                    a.DocenteCedula,
                    a.Descripcion,
                    a.FechaUtilizacion,
                    a.EstadoAscenso
                }),
                VerificacionCedulas = archivos.GroupBy(a => a.DocenteCedula)
                    .Select(g => new { Cedula = g.Key, Cantidad = g.Count() })
                    .ToList()
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Endpoint para limpiar y re-registrar archivos utilizados correctamente
    /// </summary>
    [HttpPost("limpiar-y-re-registrar/{solicitudId}")]
    public async Task<IActionResult> LimpiarYReRegistrar(int solicitudId)
    {
        try
        {
            var solicitud = await _solicitudService.GetSolicitudByIdAsync(solicitudId);
            if (solicitud == null)
            {
                return NotFound($"Solicitud {solicitudId} no encontrada");
            }

            // Esto requeriría un método adicional en el servicio para eliminar archivos
            // Por ahora, solo mostramos la información actual
            var archivosActuales = await _archivosService.ObtenerArchivosPorSolicitud(solicitudId);
            
            return Ok(new
            {
                Mensaje = "Para limpiar y re-registrar:",
                PasosSugeridos = new[]
                {
                    $"1. Ejecutar SQL: DELETE FROM ArchivosUtilizadosEscalafon WHERE SolicitudEscalafonId = {solicitudId}",
                    $"2. Llamar POST /api/debug/simular-registro/{solicitudId}",
                    "3. Verificar que solo se registren los documentos mínimos necesarios"
                },
                SolicitudInfo = new
                {
                    solicitud.Id,
                    solicitud.DocenteCedula,
                    solicitud.NivelActual,
                    solicitud.NivelSolicitado
                },
                ArchivosActuales = archivosActuales.Count,
                RequisitosTeoricos = new
                {
                    InvestigacionesNecesarias = 2,
                    EvaluacionesNecesarias = 3,
                    HorasCapacitacionNecesarias = 80
                }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene el historial con verificación de requisitos integrada
    /// </summary>
    [HttpGet("historial-con-verificacion/{cedula}")]
    public async Task<IActionResult> GetHistorialConVerificacion(string cedula)
    {
        try
        {
            // Obtener historial completo
            var historial = await _solicitudService.GetHistorialEscalafonAsync(cedula);
            
            // Para cada registro del historial, agregar la verificación de requisitos
            var historialEnriquecido = new List<object>();
            
            foreach (var registro in historial)
            {
                object verificacionRequisitos = null;
                
                try
                {
                    // Llamar a la API de verificación de requisitos
                    using var httpClient = new HttpClient();
                    httpClient.BaseAddress = new Uri(Request.Scheme + "://" + Request.Host);
                    
                    var response = await httpClient.GetAsync($"/api/EvaluacionesDesempeno/estadisticas-docente/{cedula}");
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        verificacionRequisitos = JsonSerializer.Deserialize<object>(responseContent);
                    }
                    else
                    {
                        verificacionRequisitos = new
                        {
                            error = "No se pudo obtener la verificación de requisitos",
                            status = (int)response.StatusCode
                        };
                    }
                }
                catch (Exception ex)
                {
                    verificacionRequisitos = new
                    {
                        error = $"Error al consultar API de verificación: {ex.Message}"
                    };
                }
                
                // Crear registro enriquecido
                var registroEnriquecido = new
                {
                    HistorialBase = new
                    {
                        registro.Id,
                        registro.NivelAnterior,
                        registro.NivelNuevo,
                        registro.FechaPromocion,
                        registro.EstadoSolicitud,
                        DocumentosUtilizadosCount = registro.DocumentosUtilizados?.Count ?? 0,
                        registro.ObservacionesFinales,
                        registro.AprobadoPor
                    },
                    DocumentosDetalles = registro.DocumentosDetalles,
                    VerificacionRequisitosActual = verificacionRequisitos
                };
                
                historialEnriquecido.Add(registroEnriquecido);
            }
            
            return Ok(new
            {
                CedulaConsultada = cedula,
                FechaConsulta = DateTime.Now,
                TotalRegistros = historial.Count(),
                HistorialConVerificacion = historialEnriquecido,
                Metadatos = new
                {
                    IncluirVerificacionRequisitos = true,
                    ApiVerificacionUtilizada = "/api/EvaluacionesDesempeno/estadisticas-docente/{cedula}",
                    Descripcion = "Historial de escalafones con verificación de requisitos actual para cada registro"
                }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { 
                Error = ex.Message, 
                StackTrace = ex.StackTrace,
                Endpoint = "historial-con-verificacion"
            });
        }
    }

    /// <summary>
    /// Depura el historial de escalafones para identificar problemas de duplicación o cédulas incorrectas
    /// </summary>
    [HttpGet("historial-debug/{cedula}")]
    public async Task<IActionResult> DepurarHistorial(string cedula)
    {
        try
        {
            // Obtener historial completo
            var historial = await _solicitudService.GetHistorialEscalafonAsync(cedula);
            
            // Obtener también los archivos utilizados directamente
            var archivosUtilizados = await _archivosService.ObtenerHistorialArchivos(cedula);
            
            return Ok(new
            {
                CedulaSolicitada = cedula,
                ResumenHistorial = new
                {
                    TotalRegistrosHistorial = historial.Count(),
                    RegistrosHistorial = historial.Select(h => new
                    {
                        h.Id,
                        h.NivelAnterior,
                        h.NivelNuevo,
                        h.FechaPromocion,
                        h.EstadoSolicitud,
                        DocumentosUtilizadosCount = h.DocumentosUtilizados?.Count ?? 0,
                        TieneDocumentosDetalles = h.DocumentosDetalles != null,
                        InvestigacionesCount = h.DocumentosDetalles?.Investigaciones?.Count ?? 0,
                        EvaluacionesCount = h.DocumentosDetalles?.Evaluaciones?.Count ?? 0,
                        CapacitacionesCount = h.DocumentosDetalles?.Capacitaciones?.Count ?? 0
                    }).ToList()
                },
                ResumenArchivosUtilizados = new
                {
                    TotalArchivos = archivosUtilizados.Count,
                    ArchivosPorSolicitud = archivosUtilizados
                        .GroupBy(a => a.SolicitudEscalafonId)
                        .Select(g => new
                        {
                            SolicitudId = g.Key,
                            TotalArchivos = g.Count(),
                            TiposRecurso = g.GroupBy(a => a.TipoRecurso)
                                .Select(tr => new { Tipo = tr.Key, Cantidad = tr.Count() })
                                .ToList(),
                            CedulasEncontradas = g.Select(a => a.DocenteCedula).Distinct().ToList()
                        }).ToList()
                },
                VerificacionDuplicados = new
                {
                    SolicitudesDuplicadas = historial
                        .GroupBy(h => new { h.NivelAnterior, h.NivelNuevo, h.FechaPromocion })
                        .Where(g => g.Count() > 1)
                        .Select(g => new { Grupo = g.Key, Cantidad = g.Count() })
                        .ToList(),
                    ArchivosConCedulaIncorrecta = archivosUtilizados
                        .Where(a => a.DocenteCedula != cedula)
                        .Select(a => new { a.Id, a.DocenteCedula, a.SolicitudEscalafonId })
                        .ToList()
                }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message, StackTrace = ex.StackTrace });
        }
    }
}
