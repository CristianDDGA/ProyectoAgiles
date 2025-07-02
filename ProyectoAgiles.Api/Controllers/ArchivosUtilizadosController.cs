using Microsoft.AspNetCore.Mvc;
using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Application.Interfaces;

namespace ProyectoAgiles.Api.Controllers;

/// <summary>
/// Controlador para gestionar archivos utilizados en escalafones
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("Archivos Utilizados en Escalafones")]
public class ArchivosUtilizadosController : ControllerBase
{
    private readonly IArchivosUtilizadosService _archivosUtilizadosService;

    public ArchivosUtilizadosController(IArchivosUtilizadosService archivosUtilizadosService)
    {
        _archivosUtilizadosService = archivosUtilizadosService;
    }

    /// <summary>
    /// Obtiene el historial de archivos utilizados por un docente
    /// </summary>
    /// <param name="cedula">Cédula del docente</param>
    /// <returns>Lista de archivos utilizados en ascensos previos</returns>
    [HttpGet("historial/{cedula}")]
    public async Task<ActionResult<List<ArchivosUtilizadosDto>>> ObtenerHistorialArchivos(string cedula)
    {
        try
        {
            var historial = await _archivosUtilizadosService.ObtenerHistorialArchivos(cedula);
            return Ok(historial);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene las investigaciones ya utilizadas en ascensos previos
    /// </summary>
    /// <param name="cedula">Cédula del docente</param>
    /// <returns>Lista de IDs de investigaciones utilizadas</returns>
    [HttpGet("investigaciones-utilizadas/{cedula}")]
    public async Task<ActionResult<List<int>>> ObtenerInvestigacionesUtilizadas(string cedula)
    {
        try
        {
            var investigacionesUtilizadas = await _archivosUtilizadosService.ObtenerInvestigacionesUtilizadas(cedula);
            return Ok(investigacionesUtilizadas);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene las evaluaciones ya utilizadas en ascensos previos
    /// </summary>
    /// <param name="cedula">Cédula del docente</param>
    /// <returns>Lista de IDs de evaluaciones utilizadas</returns>
    [HttpGet("evaluaciones-utilizadas/{cedula}")]
    public async Task<ActionResult<List<int>>> ObtenerEvaluacionesUtilizadas(string cedula)
    {
        try
        {
            var evaluacionesUtilizadas = await _archivosUtilizadosService.ObtenerEvaluacionesUtilizadas(cedula);
            return Ok(evaluacionesUtilizadas);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene las capacitaciones ya utilizadas en ascensos previos
    /// </summary>
    /// <param name="cedula">Cédula del docente</param>
    /// <returns>Lista de IDs de capacitaciones utilizadas</returns>
    [HttpGet("capacitaciones-utilizadas/{cedula}")]
    public async Task<ActionResult<List<int>>> ObtenerCapacitacionesUtilizadas(string cedula)
    {
        try
        {
            var capacitacionesUtilizadas = await _archivosUtilizadosService.ObtenerCapacitacionesUtilizadas(cedula);
            return Ok(capacitacionesUtilizadas);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// Verifica si un archivo específico ya fue utilizado en un ascenso previo
    /// </summary>
    /// <param name="cedula">Cédula del docente</param>
    /// <param name="tipoRecurso">Tipo de recurso (Investigacion, EvaluacionDesempeno, Capacitacion)</param>
    /// <param name="recursoId">ID del recurso</param>
    /// <returns>True si el archivo ya fue utilizado</returns>
    [HttpGet("verificar-utilizado/{cedula}/{tipoRecurso}/{recursoId}")]
    public async Task<ActionResult<bool>> VerificarArchivoUtilizado(string cedula, string tipoRecurso, int recursoId)
    {
        try
        {
            var yaUtilizado = await _archivosUtilizadosService.ArchivoYaUtilizado(cedula, tipoRecurso, recursoId);
            return Ok(yaUtilizado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene estadísticas de archivos utilizados por tipo
    /// </summary>
    /// <param name="cedula">Cédula del docente</param>
    /// <returns>Diccionario con estadísticas por tipo de recurso</returns>
    [HttpGet("estadisticas/{cedula}")]
    public async Task<ActionResult<Dictionary<string, int>>> ObtenerEstadisticasArchivos(string cedula)
    {
        try
        {
            var estadisticas = await _archivosUtilizadosService.ObtenerEstadisticasArchivosUtilizados(cedula);
            return Ok(estadisticas);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene un resumen completo de archivos utilizados por un docente
    /// </summary>
    /// <param name="cedula">Cédula del docente</param>
    /// <returns>Resumen completo con historial y estadísticas</returns>
    [HttpGet("resumen/{cedula}")]
    public async Task<ActionResult<ResumenArchivosUtilizadosDto>> ObtenerResumenCompleto(string cedula)
    {
        try
        {
            var historial = await _archivosUtilizadosService.ObtenerHistorialArchivos(cedula);
            var estadisticas = await _archivosUtilizadosService.ObtenerEstadisticasArchivosUtilizados(cedula);

            var resumen = new ResumenArchivosUtilizadosDto
            {
                DocenteCedula = cedula,
                TotalInvestigacionesUtilizadas = estadisticas.GetValueOrDefault("Investigacion", 0),
                TotalEvaluacionesUtilizadas = estadisticas.GetValueOrDefault("EvaluacionDesempeno", 0),
                TotalCapacitacionesUtilizadas = estadisticas.GetValueOrDefault("Capacitacion", 0),
                TotalAscensosCompletados = historial.Where(h => h.EstadoAscenso == "Aprobado").Select(h => h.SolicitudEscalafonId).Distinct().Count(),
                HistorialCompleto = historial,
                EstadisticasPorTipo = estadisticas
            };

            return Ok(resumen);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }
}
