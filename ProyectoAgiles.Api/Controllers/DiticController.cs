using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Application.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ProyectoAgiles.Api.Controllers;

/// <summary>
/// Controlador para la gestión de capacitaciones DITIC
/// </summary>
[ApiController]
[Route("api/[controller]")]
// [Authorize] // Comentado temporalmente para pruebas
public class DiticController : ControllerBase
{
    private readonly IDiticService _diticService;
    private readonly ILogger<DiticController> _logger;

    public DiticController(IDiticService diticService, ILogger<DiticController> logger)
    {
        _diticService = diticService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las capacitaciones DITIC
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DiticDto>>> GetAll()
    {
        try
        {
            var capacitaciones = await _diticService.GetAllAsync();
            return Ok(capacitaciones);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las capacitaciones DITIC");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene una capacitación DITIC por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<DiticDto>> GetById(int id)
    {
        try
        {
            var capacitacion = await _diticService.GetByIdAsync(id);
            if (capacitacion == null)
                return NotFound($"No se encontró la capacitación con ID {id}");

            return Ok(capacitacion);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la capacitación DITIC con ID {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene las capacitaciones DITIC de un docente por cédula
    /// </summary>
    [HttpGet("docente/{cedula}")]
    public async Task<ActionResult<IEnumerable<DiticDto>>> GetByCedula(
        [Required] string cedula)
    {
        try
        {
            var capacitaciones = await _diticService.GetByCedulaAsync(cedula);
            return Ok(capacitaciones);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener capacitaciones DITIC para la cédula {Cedula}", cedula);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene capacitaciones DITIC disponibles (no utilizadas) para escalafón
    /// </summary>
    [HttpGet("disponibles/{cedula}")]
    public async Task<ActionResult<IEnumerable<DiticDto>>> GetDisponibles(
        [Required] string cedula)
    {
        try
        {
            var capacitaciones = await _diticService.GetDisponiblesParaEscalafonAsync(cedula);
            return Ok(capacitaciones);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener capacitaciones DITIC disponibles para la cédula {Cedula}", cedula);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene las capacitaciones DITIC de los últimos 3 años por cédula
    /// </summary>
    [HttpGet("docente/{cedula}/ultimos-tres-anos")]
    public async Task<ActionResult<IEnumerable<DiticDto>>> GetByCedulaLastThreeYears(
        [Required] string cedula)
    {
        try
        {
            var capacitaciones = await _diticService.GetByCedulaLastThreeYearsAsync(cedula);
            return Ok(capacitaciones);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener capacitaciones DITIC de los últimos 3 años para la cédula {Cedula}", cedula);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Crea una nueva capacitación DITIC
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<DiticDto>> Create([FromBody] CreateDiticDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var capacitacion = await _diticService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = capacitacion.Id }, capacitacion);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Error de validación al crear capacitación DITIC");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear capacitación DITIC");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Crea una nueva capacitación DITIC con certificado PDF
    /// </summary>
    [HttpPost("con-certificado")]
    public async Task<ActionResult<DiticDto>> CreateWithPdf([FromForm] CreateDiticWithPdfDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validar archivo PDF
            if (createDto.ArchivoCertificado != null)
            {
                if (createDto.ArchivoCertificado.Length > 10 * 1024 * 1024) // 10MB máximo
                    return BadRequest("El archivo PDF no puede superar los 10MB");

                if (!createDto.ArchivoCertificado.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
                    return BadRequest("Solo se permiten archivos PDF");
            }

            var capacitacion = await _diticService.CreateWithPdfAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = capacitacion.Id }, capacitacion);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Error de validación al crear capacitación DITIC con PDF");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear capacitación DITIC con PDF");
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Actualiza una capacitación DITIC existente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<DiticDto>> Update(int id, [FromBody] UpdateDiticDto updateDto)
    {
        try
        {
            if (id != updateDto.Id)
                return BadRequest("El ID de la URL no coincide con el ID del objeto");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var capacitacion = await _diticService.UpdateAsync(updateDto);
            return Ok(capacitacion);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Error de validación al actualizar capacitación DITIC con ID {Id}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar capacitación DITIC con ID {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Elimina una capacitación DITIC
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _diticService.DeleteAsync(id);
            if (!deleted)
                return NotFound($"No se encontró la capacitación con ID {id}");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar capacitación DITIC con ID {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Verifica el cumplimiento del requisito DITIC para un docente
    /// </summary>
    [HttpGet("verificar-requisito/{cedula}")]
    public async Task<ActionResult<VerificacionRequisitoDiticDto>> VerifyRequirement(
        [Required] string cedula)
    {
        try
        {
            var verificacion = await _diticService.VerifyRequirementAsync(cedula);
            return Ok(verificacion);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar requisito DITIC para la cédula {Cedula}", cedula);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene el resumen de capacitaciones de un docente
    /// </summary>
    [HttpGet("resumen/{cedula}")]
    public async Task<ActionResult<ResumenCapacitacionesDto>> GetSummary(
        [Required] string cedula)
    {
        try
        {
            var resumen = await _diticService.GetSummaryByCedulaAsync(cedula);
            return Ok(resumen);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener resumen DITIC para la cédula {Cedula}", cedula);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Obtiene estadísticas de capacitaciones por tipo
    /// </summary>
    [HttpGet("estadisticas/{cedula}")]
    public async Task<ActionResult<IEnumerable<EstadisticasCapacitacionDto>>> GetStatistics(
        [Required] string cedula)
    {
        try
        {
            var estadisticas = await _diticService.GetStatisticsByTypeAsync(cedula);
            return Ok(estadisticas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener estadísticas DITIC para la cédula {Cedula}", cedula);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Descarga el certificado PDF de una capacitación
    /// </summary>
    [HttpGet("{id}/certificado")]
    public async Task<ActionResult> DownloadCertificate(int id)
    {
        try
        {
            var capacitacion = await _diticService.GetByIdAsync(id);
            if (capacitacion == null)
                return NotFound($"No se encontró la capacitación con ID {id}");

            var pdfContent = await _diticService.GetCertificatePdfAsync(id);
            if (pdfContent == null || pdfContent.Length == 0)
                return NotFound("No se encontró el certificado PDF");

            var fileName = !string.IsNullOrEmpty(capacitacion.NombreArchivoCertificado) 
                ? capacitacion.NombreArchivoCertificado 
                : $"certificado_ditic_{id}.pdf";

            return File(pdfContent, "application/pdf", fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al descargar certificado DITIC con ID {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Actualiza el certificado PDF de una capacitación
    /// </summary>
    [HttpPut("{id}/certificado")]
    public async Task<ActionResult> UpdateCertificate(int id, IFormFile archivo)
    {
        try
        {
            if (archivo == null || archivo.Length == 0)
                return BadRequest("No se proporcionó ningún archivo");

            if (archivo.Length > 10 * 1024 * 1024) // 10MB máximo
                return BadRequest("El archivo PDF no puede superar los 10MB");

            if (!archivo.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Solo se permiten archivos PDF");

            using var memoryStream = new MemoryStream();
            await archivo.CopyToAsync(memoryStream);
            var pdfContent = memoryStream.ToArray();

            var updated = await _diticService.UpdateCertificatePdfAsync(id, pdfContent, archivo.FileName);
            if (!updated)
                return NotFound($"No se encontró la capacitación con ID {id}");

            return Ok(new { message = "Certificado actualizado correctamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar certificado DITIC con ID {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Elimina el certificado PDF de una capacitación
    /// </summary>
    [HttpDelete("{id}/certificado")]
    public async Task<ActionResult> DeleteCertificate(int id)
    {
        try
        {
            var deleted = await _diticService.DeleteCertificatePdfAsync(id);
            if (!deleted)
                return NotFound($"No se encontró la capacitación con ID {id}");

            return Ok(new { message = "Certificado eliminado correctamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar certificado DITIC con ID {Id}", id);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Importa capacitaciones desde el sistema externo
    /// </summary>
    [HttpPost("importar/{cedula}")]
    public async Task<ActionResult> ImportFromExternalSystem([Required] string cedula)
    {
        try
        {
            var isValid = await _diticService.ValidateExternalDataAsync(cedula);
            if (!isValid)
                return BadRequest("La cédula no es válida o no se encontraron datos externos");

            var importedCount = await _diticService.ImportFromExternalSystemAsync(cedula);
            
            if (importedCount == 0)
                return Ok(new { message = "No se encontraron nuevas capacitaciones para importar", imported = 0 });

            return Ok(new 
            { 
                message = $"Se importaron {importedCount} capacitaciones correctamente", 
                imported = importedCount 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al importar capacitaciones DITIC para la cédula {Cedula}", cedula);
            return StatusCode(500, "Error interno del servidor");
        }
    }

    /// <summary>
    /// Busca capacitaciones con filtros avanzados
    /// </summary>
    [HttpGet("buscar")]
    public async Task<ActionResult<IEnumerable<DiticDto>>> Search(
        [FromQuery] string? searchTerm,
        [FromQuery] string? cedula,
        [FromQuery] string? tipo,
        [FromQuery] string? institucion,
        [FromQuery] int? año,
        [FromQuery] string? estado)
    {
        try
        {
            var capacitaciones = await _diticService.SearchAsync(searchTerm, cedula, tipo, institucion, año, estado);
            return Ok(capacitaciones);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar capacitaciones DITIC con filtros");
            return StatusCode(500, "Error interno del servidor");
        }
    }
}
