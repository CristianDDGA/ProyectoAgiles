using Microsoft.AspNetCore.Mvc;
using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Application.Interfaces;

namespace ProyectoAgiles.Api.Controllers;

/// <summary>
/// Controlador para la gestión de solicitudes de escalafón
/// </summary>
[ApiController]
[Route("api/solicitudes-escalafon")]
public class SolicitudesEscalafonController : ControllerBase
{
    private readonly ISolicitudEscalafonService _solicitudService;
    private readonly ILogger<SolicitudesEscalafonController> _logger;

    public SolicitudesEscalafonController(
        ISolicitudEscalafonService solicitudService,
        ILogger<SolicitudesEscalafonController> logger)
    {
        _solicitudService = solicitudService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las solicitudes de escalafón
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SolicitudEscalafonDto>>> GetAllSolicitudes()
    {
        try
        {
            var solicitudes = await _solicitudService.GetAllSolicitudesAsync();
            return Ok(solicitudes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener solicitudes de escalafón");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene una solicitud específica por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<SolicitudEscalafonDto>> GetSolicitudById(int id)
    {
        try
        {
            var solicitud = await _solicitudService.GetSolicitudByIdAsync(id);
            if (solicitud == null)
            {
                return NotFound("Solicitud no encontrada");
            }
            return Ok(solicitud);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener solicitud {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene solicitudes por cédula del docente
    /// </summary>
    [HttpGet("docente/{cedula}")]
    public async Task<ActionResult<IEnumerable<SolicitudEscalafonDto>>> GetSolicitudesByCedula(string cedula)
    {
        try
        {
            var solicitudes = await _solicitudService.GetSolicitudesByCedulaAsync(cedula);
            return Ok(solicitudes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener solicitudes para cédula {Cedula}", cedula);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene solicitudes por estado
    /// </summary>
    [HttpGet("estado/{status}")]
    public async Task<ActionResult<IEnumerable<SolicitudEscalafonDto>>> GetSolicitudesByStatus(string status)
    {
        try
        {
            var solicitudes = await _solicitudService.GetSolicitudesByStatusAsync(status);
            return Ok(solicitudes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener solicitudes por estado {Status}", status);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene el conteo de solicitudes pendientes
    /// </summary>
    [HttpGet("pendientes/count")]
    public async Task<ActionResult<int>> GetPendingCount()
    {
        try
        {
            var count = await _solicitudService.GetPendingCountAsync();
            return Ok(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener conteo de solicitudes pendientes");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene el conteo de solicitudes pendientes (endpoint alternativo)
    /// </summary>
    [HttpGet("pending-count")]
    public async Task<ActionResult<int>> GetPendingCountAlternative()
    {
        try
        {
            var count = await _solicitudService.GetPendingCountAsync();
            return Ok(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener conteo de solicitudes pendientes");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Crea una nueva solicitud de escalafón
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SolicitudEscalafonDto>> CreateSolicitud([FromBody] CreateSolicitudEscalafonDto createDto)
    {
        try
        {
            var solicitud = await _solicitudService.CreateSolicitudAsync(createDto);
            return CreatedAtAction(nameof(GetSolicitudById), new { id = solicitud.Id }, solicitud);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear solicitud de escalafón");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Actualiza el estado de una solicitud
    /// </summary>
    [HttpPut("estado")]
    public async Task<ActionResult<SolicitudEscalafonDto>> UpdateSolicitudStatus([FromBody] UpdateSolicitudStatusDto updateDto)
    {
        try
        {
            var solicitud = await _solicitudService.UpdateSolicitudStatusAsync(updateDto);
            return Ok(solicitud);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar estado de solicitud {Id}", updateDto.Id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Actualiza el estado de una solicitud específica por ID
    /// </summary>
    [HttpPut("{id}/status")]
    public async Task<ActionResult<SolicitudEscalafonDto>> UpdateSolicitudStatusById(int id, [FromBody] UpdateSolicitudStatusDto updateDto)
    {
        try
        {
            // Asegurar que el ID del DTO coincida con el ID de la ruta
            updateDto.Id = id;
            
            var solicitud = await _solicitudService.UpdateSolicitudStatusAsync(updateDto);
            return Ok(solicitud);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar estado de solicitud {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Elimina una solicitud (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSolicitud(int id)
    {
        try
        {
            var result = await _solicitudService.DeleteSolicitudAsync(id);
            if (!result)
            {
                return NotFound("Solicitud no encontrada");
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar solicitud {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Verifica si existe una solicitud pendiente para una cédula
    /// </summary>
    [HttpGet("existe-pendiente/{cedula}")]
    public async Task<ActionResult<bool>> ExisteSolicitudPendiente(string cedula)
    {
        try
        {
            var existe = await _solicitudService.ExisteSolicitudPendienteAsync(cedula);
            return Ok(existe);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar solicitud pendiente para cédula {Cedula}", cedula);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Notifica por correo electrónico la aprobación de una solicitud
    /// </summary>
    [HttpPost("{id}/notificar-aprobacion")]
    public async Task<ActionResult> NotificarAprobacion(int id)
    {
        try
        {
            var resultado = await _solicitudService.NotificarAprobacionAsync(id);
            if (!resultado)
            {
                return BadRequest("No se pudo enviar la notificación. Verifique que la solicitud exista y tenga un email válido.");
            }
            return Ok(new { mensaje = "Notificación enviada exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al notificar aprobación para solicitud {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Finaliza el proceso de escalafón, actualizando el nivel del docente
    /// </summary>
    [HttpPost("{id}/finalizar-escalafon")]
    public async Task<ActionResult> FinalizarEscalafon(int id)
    {
        try
        {
            var resultado = await _solicitudService.FinalizarEscalafonAsync(id);
            if (!resultado)
            {
                return BadRequest("No se pudo finalizar el escalafón. Verifique que la solicitud y el docente existan.");
            }
            return Ok(new { mensaje = "Escalafón finalizado exitosamente. Se ha actualizado el nivel del docente y se ha enviado la notificación." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al finalizar escalafón para solicitud {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Rechaza una solicitud con motivo y envía notificación por correo
    /// </summary>
    [HttpPost("{id}/rechazar")]
    public async Task<ActionResult<SolicitudEscalafonDto>> RechazarSolicitud(int id, [FromBody] RechazarSolicitudDto rechazarDto)
    {
        try
        {
            var solicitud = await _solicitudService.RechazarSolicitudAsync(id, rechazarDto.MotivoRechazo, rechazarDto.RechazadoPor, rechazarDto.NivelRechazo);
            return Ok(solicitud);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al rechazar solicitud {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Crea una apelación para una solicitud rechazada
    /// </summary>
    [HttpPost("{id}/apelar")]
    public async Task<ActionResult<SolicitudEscalafonDto>> CrearApelacion(int id, [FromBody] CrearApelacionDto apelacionDto)
    {
        try
        {
            var nuevaSolicitud = await _solicitudService.CrearApelacionAsync(id, apelacionDto.ObservacionesApelacion);
            return CreatedAtAction(nameof(GetSolicitudById), new { id = nuevaSolicitud.Id }, nuevaSolicitud);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear apelación para solicitud {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }
}

/// <summary>
/// DTO para rechazar una solicitud
/// </summary>
public class RechazarSolicitudDto
{
    public string MotivoRechazo { get; set; } = string.Empty;
    public string RechazadoPor { get; set; } = string.Empty;
    public string NivelRechazo { get; set; } = string.Empty;
}

/// <summary>
/// DTO para crear una apelación
/// </summary>
public class CrearApelacionDto
{
    public string ObservacionesApelacion { get; set; } = string.Empty;
}
