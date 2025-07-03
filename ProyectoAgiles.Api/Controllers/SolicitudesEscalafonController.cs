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
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IEmailService _emailService;

    public SolicitudesEscalafonController(
        ISolicitudEscalafonService solicitudService,
        ILogger<SolicitudesEscalafonController> logger,
        IWebHostEnvironment webHostEnvironment,
        IEmailService emailService)
    {
        _solicitudService = solicitudService;
        _logger = logger;
        _webHostEnvironment = webHostEnvironment;
        _emailService = emailService;
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
    public async Task<ActionResult<SolicitudEscalafonDto>> CreateSolicitud([FromBody] ProyectoAgiles.Application.DTOs.CreateSolicitudEscalafonDto createDto)
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
    public async Task<ActionResult<SolicitudEscalafonDto>> UpdateSolicitudStatus([FromBody] ProyectoAgiles.Application.DTOs.UpdateSolicitudStatusDto updateDto)
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
    public async Task<ActionResult<SolicitudEscalafonDto>> UpdateSolicitudStatusById(int id, [FromBody] ProyectoAgiles.Application.DTOs.UpdateSolicitudStatusDto updateDto)
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
    public async Task<ActionResult<SolicitudEscalafonDto>> CrearApelacion(int id, [FromForm] CrearApelacionDto apelacionDto)
    {
        try
        {
            var nuevaSolicitud = await _solicitudService.CrearApelacionAsync(id, apelacionDto.ObservacionesApelacion, apelacionDto.Destinatario, apelacionDto.Archivos);
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

    /// <summary>
    /// Obtiene un archivo de apelación para visualizar en el navegador
    /// </summary>
    [HttpGet("archivo/{id:int}/{nombreArchivo}")]
    public async Task<IActionResult> ObtenerArchivoApelacion(int id, string nombreArchivo)
    {
        try
        {
            _logger.LogInformation("Solicitando archivo de apelación: {NombreArchivo} para solicitud {Id}", nombreArchivo, id);
            _logger.LogInformation("WebRootPath: {WebRootPath}", _webHostEnvironment.WebRootPath);

            // Verificar que la solicitud existe
            var solicitud = await _solicitudService.GetSolicitudByIdAsync(id);
            if (solicitud == null)
            {
                _logger.LogWarning("Solicitud {Id} no encontrada", id);
                return NotFound($"Solicitud {id} no encontrada");
            }

            // Construir la ruta del archivo
            var uploadsPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "apelaciones", id.ToString());
            var archivoPath = Path.Combine(uploadsPath, nombreArchivo);

            _logger.LogInformation("Buscando archivo en ruta: {ArchivoPath}", archivoPath);

            // Verificar que el archivo existe
            if (!System.IO.File.Exists(archivoPath))
            {
                _logger.LogWarning("Archivo no encontrado: {ArchivoPath}", archivoPath);
                return NotFound($"Archivo '{nombreArchivo}' no encontrado para la solicitud {id}");
            }

            // Leer el archivo
            var archivoBytes = await System.IO.File.ReadAllBytesAsync(archivoPath);
            
            // Determinar el tipo de contenido basado en la extensión
            var contentType = GetContentType(nombreArchivo);
            
            _logger.LogInformation("Archivo encontrado. Tamaño: {Tamaño} bytes, Tipo: {ContentType}", archivoBytes.Length, contentType);

            // Configurar headers para visualización en línea (no descarga)
            Response.Headers.Append("Content-Disposition", $"inline; filename=\"{nombreArchivo}\"");
            Response.Headers.Append("X-Content-Type-Options", "nosniff");
            
            return File(archivoBytes, contentType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener archivo de apelación {NombreArchivo} para solicitud {Id}", nombreArchivo, id);
            return StatusCode(500, $"Error al obtener el archivo: {ex.Message}");
        }
    }

    /// <summary>
    /// Determina el tipo de contenido basado en la extensión del archivo
    /// </summary>
    private static string GetContentType(string nombreArchivo)
    {
        var extension = Path.GetExtension(nombreArchivo).ToLowerInvariant();
        return extension switch
        {
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".txt" => "text/plain",
            ".html" => "text/html",
            _ => "application/octet-stream"
        };
    }

    /// <summary>
    /// Acepta una apelación y reinicia el ciclo de evaluación
    /// </summary>
    [HttpPost("{id}/apelacion/aceptar")]
    public async Task<ActionResult<SolicitudEscalafonDto>> AceptarApelacion(int id, [FromBody] AceptarApelacionDto aceptarDto)
    {
        try
        {
            var solicitud = await _solicitudService.GetSolicitudByIdAsync(id);
            if (solicitud == null)
            {
                return NotFound($"Solicitud {id} no encontrada");
            }

            // Verificar que sea una apelación pendiente
            if (solicitud.Status != "PendienteComision")
            {
                return BadRequest("La solicitud no es una apelación pendiente de evaluación");
            }

            // Cambiar estado a pendiente para reiniciar el ciclo
            var updateDto = new ProyectoAgiles.Application.DTOs.UpdateSolicitudStatusDto
            {
                Id = id,
                Status = "Pendiente",
                MotivoRechazo = $"APELACIÓN ACEPTADA por {aceptarDto.AceptadoPor}: {aceptarDto.ObservacionesAceptacion}",
                ProcesadoPor = aceptarDto.AceptadoPor
            };

            var solicitudActualizada = await _solicitudService.UpdateSolicitudStatusAsync(updateDto);
            
            // Enviar notificación por correo al docente
            try
            {
                await _emailService.SendApelacionAceptadaEmailAsync(
                    solicitud.DocenteEmail, 
                    solicitud.DocenteNombre, 
                    aceptarDto.ObservacionesAceptacion,
                    aceptarDto.AceptadoPor);
                    
                _logger.LogInformation("Notificación de aceptación de apelación enviada para solicitud {Id}", id);
            }
            catch (Exception emailEx)
            {
                _logger.LogWarning(emailEx, "Error al enviar notificación de aceptación de apelación para solicitud {Id}", id);
                // No fallar la operación por problemas de email
            }
            
            _logger.LogInformation("Apelación aceptada para solicitud {Id} por {Usuario}", id, aceptarDto.AceptadoPor);
            
            return Ok(solicitudActualizada);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al aceptar apelación para solicitud {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Rechaza una apelación definitivamente y notifica al docente
    /// </summary>
    [HttpPost("{id}/apelacion/rechazar")]
    public async Task<ActionResult<SolicitudEscalafonDto>> RechazarApelacion(int id, [FromBody] RechazarApelacionDto rechazarDto)
    {
        try
        {
            var solicitud = await _solicitudService.GetSolicitudByIdAsync(id);
            if (solicitud == null)
            {
                return NotFound($"Solicitud {id} no encontrada");
            }

            // Verificar que sea una apelación pendiente
            if (solicitud.Status != "PendienteComision")
            {
                return BadRequest("La solicitud no es una apelación pendiente de evaluación");
            }

            // Cambiar estado a rechazado definitivamente
            var updateDto = new ProyectoAgiles.Application.DTOs.UpdateSolicitudStatusDto
            {
                Id = id,
                Status = "RechazadoDefinitivo",
                MotivoRechazo = $"APELACIÓN RECHAZADA por {rechazarDto.RechazadoPor}: {rechazarDto.MotivoRechazoApelacion}",
                ProcesadoPor = rechazarDto.RechazadoPor
            };

            var solicitudActualizada = await _solicitudService.UpdateSolicitudStatusAsync(updateDto);

            // Enviar notificación por correo al docente
            try
            {
                await _emailService.SendApelacionRechazoEmailAsync(
                    solicitud.DocenteEmail, 
                    solicitud.DocenteNombre, 
                    rechazarDto.MotivoRechazoApelacion,
                    rechazarDto.RechazadoPor);
                    
                _logger.LogInformation("Notificación de rechazo de apelación enviada para solicitud {Id}", id);
            }
            catch (Exception emailEx)
            {
                _logger.LogWarning(emailEx, "Error al enviar notificación de rechazo de apelación para solicitud {Id}", id);
                // No fallar la operación por problemas de email
            }
            
            _logger.LogInformation("Apelación rechazada para solicitud {Id} por {Usuario}", id, rechazarDto.RechazadoPor);
            
            return Ok(solicitudActualizada);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al rechazar apelación para solicitud {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene el historial de escalafones completados de un docente
    /// </summary>
    [HttpGet("~/api/escalafon/historial/{cedula}")]
    public async Task<ActionResult<IEnumerable<HistorialEscalafonDto>>> GetHistorialEscalafon(string cedula)
    {
        try
        {
            _logger.LogInformation("Obteniendo historial para cédula: {Cedula}", cedula);
            var historial = await _solicitudService.GetHistorialEscalafonAsync(cedula);
            _logger.LogInformation("Historial obtenido: {Count} registros para cédula {Cedula}", historial.Count(), cedula);
            return Ok(historial);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener historial de escalafón para docente {Cedula}", cedula);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Endpoint de debug para obtener todos los archivos utilizados por solicitud específica
    /// </summary>
    [HttpGet("~/api/debug/archivos-solicitud/{solicitudId}")]
    public async Task<ActionResult> GetArchivosPorSolicitudDebug(int solicitudId)
    {
        try
        {
            _logger.LogInformation("DEBUG: Obteniendo archivos para solicitud {SolicitudId}", solicitudId);
            
            // Usar el servicio directamente
            var archivos = await _solicitudService.GetSolicitudByIdAsync(solicitudId);
            if (archivos == null)
            {
                return NotFound($"Solicitud {solicitudId} no encontrada");
            }
            
            return Ok(new
            {
                SolicitudId = solicitudId,
                SolicitudEncontrada = true,
                DocenteCedula = archivos.DocenteCedula ?? "N/A",
                Status = archivos.Status ?? "N/A",
                NivelActual = archivos.NivelActual ?? "N/A",
                NivelSolicitado = archivos.NivelSolicitado ?? "N/A"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en debug de archivos para solicitud {SolicitudId}", solicitudId);
            return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
        }
    }

}
