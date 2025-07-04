using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Application.Interfaces;

namespace ProyectoAgiles.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PeriodosPostulacionController : ControllerBase
{
    private readonly IPeriodoPostulacionService _service;

    public PeriodosPostulacionController(IPeriodoPostulacionService service)
    {
        _service = service;
    }

    /// <summary>
    /// Obtiene todos los períodos de postulación (Solo Administrador)
    /// </summary>
    [HttpGet]
    //[Authorize(Roles = "Administrador")] // Temporarily disabled for testing
    public async Task<ActionResult<ApiResponse<IEnumerable<PeriodoPostulacionDto>>>> GetAllPeriods()
    {
        var result = await _service.GetAllPeriodsAsync();
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Obtiene un período específico por ID (Solo Administrador)
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ApiResponse<PeriodoPostulacionDto>>> GetPeriodById(int id)
    {
        var result = await _service.GetPeriodByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Obtiene información del período activo (Disponible para docentes)
    /// </summary>
    [HttpGet("active-info")]
    //[Authorize] // Temporarily disabled for testing
    public async Task<ActionResult<ApiResponse<PeriodoInfoDto>>> GetActivePeriodInfo()
    {
        var result = await _service.GetActivePeriodInfoAsync();
        return Ok(result);
    }

    /// <summary>
    /// Valida si se puede crear una solicitud en el período actual (Para docentes)
    /// </summary>
    [HttpGet("validate-creation")]
    //[Authorize] // Temporarily disabled for testing
    public async Task<ActionResult<ApiResponse<bool>>> ValidateCanCreateSolicitud()
    {
        var result = await _service.ValidateCanCreateSolicitudAsync();
        return Ok(result);
    }

    /// <summary>
    /// Crea un nuevo período de postulación (Solo Administrador)
    /// </summary>
    [HttpPost]
    //[Authorize(Roles = "Administrador")] // Temporarily disabled for testing
    public async Task<ActionResult<ApiResponse<PeriodoPostulacionDto>>> CreatePeriod([FromBody] CreatePeriodoPostulacionDto createDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(new ApiResponse<PeriodoPostulacionDto>
            {
                Success = false,
                Message = "Datos inválidos",
                Errors = errors
            });
        }

        var result = await _service.CreatePeriodAsync(createDto);
        return result.Success ? CreatedAtAction(nameof(GetPeriodById), new { id = result.Data?.Id }, result) : BadRequest(result);
    }

    /// <summary>
    /// Actualiza un período existente (Solo Administrador)
    /// </summary>
    [HttpPut("{id}")]
    //[Authorize(Roles = "Administrador")] // Temporarily disabled for testing
    public async Task<ActionResult<ApiResponse<PeriodoPostulacionDto>>> UpdatePeriod(int id, [FromBody] UpdatePeriodoPostulacionDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(new ApiResponse<PeriodoPostulacionDto>
            {
                Success = false,
                Message = "Datos inválidos",
                Errors = errors
            });
        }

        var result = await _service.UpdatePeriodAsync(id, updateDto);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Elimina un período (Solo Administrador)
    /// </summary>
    [HttpDelete("{id}")]
    //[Authorize(Roles = "Administrador")] // Temporarily disabled for testing
    public async Task<ActionResult<ApiResponse<bool>>> DeletePeriod(int id)
    {
        var result = await _service.DeletePeriodAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Activa un período específico (desactiva todos los otros) (Solo Administrador)
    /// </summary>
    [HttpPost("{id}/activate")]
    //[Authorize(Roles = "Administrador")] // Temporarily disabled for testing
    public async Task<ActionResult<ApiResponse<bool>>> ActivatePeriod(int id)
    {
        var result = await _service.ActivatePeriodAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Desactiva un período específico (Solo Administrador)
    /// </summary>
    [HttpPost("{id}/deactivate")]
    //[Authorize(Roles = "Administrador")] // Temporarily disabled for testing
    public async Task<ActionResult<ApiResponse<bool>>> DeactivatePeriod(int id)
    {
        var result = await _service.DeactivatePeriodAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Desactiva todos los períodos (Solo Administrador)
    /// </summary>
    [HttpPost("deactivate-all")]
    //[Authorize(Roles = "Administrador")] // Temporarily disabled for testing
    public async Task<ActionResult<ApiResponse<bool>>> DeactivateAllPeriods()
    {
        var result = await _service.DeactivateAllPeriodsAsync();
        return Ok(result);
    }

    /// <summary>
    /// Obtiene períodos por rango de fechas (Solo Administrador)
    /// </summary>
    [HttpGet("by-date-range")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<ApiResponse<IEnumerable<PeriodoPostulacionDto>>>> GetPeriodsByDateRange(
        [FromQuery] DateTime startDate, 
        [FromQuery] DateTime endDate)
    {
        var result = await _service.GetPeriodsByDateRangeAsync(startDate, endDate);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
