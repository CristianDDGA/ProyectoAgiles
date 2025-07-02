using Microsoft.AspNetCore.Mvc;
using ProyectoAgiles.Application.Interfaces;

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
    /// Obtiene el historial completo de una cédula
    /// </summary>
    [HttpGet("historial-completo/{cedula}")]
    public async Task<IActionResult> GetHistorialCompleto(string cedula)
    {
        try
        {
            var historial = await _solicitudService.GetHistorialEscalafonAsync(cedula);
            return Ok(new
            {
                Cedula = cedula,
                TotalRegistros = historial.Count(),
                Historial = historial
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}
