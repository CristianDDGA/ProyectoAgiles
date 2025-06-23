using Microsoft.AspNetCore.Mvc;
using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Application.Interfaces;

namespace ProyectoAgiles.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvestigacionesController : ControllerBase
{
    private readonly IInvestigacionService _investigacionService;

    public InvestigacionesController(IInvestigacionService investigacionService)
    {
        _investigacionService = investigacionService;
    }

    /// <summary>
    /// Obtiene todas las investigaciones
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InvestigacionDto>>> GetAll()
    {
        try
        {
            var investigaciones = await _investigacionService.GetAllAsync();
            return Ok(investigaciones);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene una investigación por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<InvestigacionDto>> GetById(int id)
    {
        try
        {
            var investigacion = await _investigacionService.GetByIdAsync(id);
            if (investigacion == null)
                return NotFound(new { message = "Investigación no encontrada" });

            return Ok(investigacion);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene investigaciones por cédula
    /// </summary>
    [HttpGet("by-cedula/{cedula}")]
    public async Task<ActionResult<IEnumerable<InvestigacionDto>>> GetByCedula(string cedula)
    {
        try
        {
            var investigaciones = await _investigacionService.GetByCedulaAsync(cedula);
            return Ok(investigaciones);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene investigaciones por tipo
    /// </summary>
    [HttpGet("by-tipo/{tipo}")]
    public async Task<ActionResult<IEnumerable<InvestigacionDto>>> GetByTipo(string tipo)
    {
        try
        {
            var investigaciones = await _investigacionService.GetByTipoAsync(tipo);
            return Ok(investigaciones);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene investigaciones por campo de conocimiento
    /// </summary>
    [HttpGet("by-campo/{campoConocimiento}")]
    public async Task<ActionResult<IEnumerable<InvestigacionDto>>> GetByCampoConocimiento(string campoConocimiento)
    {
        try
        {
            var investigaciones = await _investigacionService.GetByCampoConocimientoAsync(campoConocimiento);
            return Ok(investigaciones);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Crea una nueva investigación
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<InvestigacionDto>> Create([FromBody] CreateInvestigacionDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var investigacion = await _investigacionService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = investigacion.Id }, investigacion);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza una investigación existente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<InvestigacionDto>> Update(int id, [FromBody] UpdateInvestigacionDto updateDto)
    {
        try
        {
            if (id != updateDto.Id)
                return BadRequest(new { message = "El ID de la URL no coincide con el ID del objeto" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var exists = await _investigacionService.ExistsAsync(id);
            if (!exists)
                return NotFound(new { message = "Investigación no encontrada" });

            var investigacion = await _investigacionService.UpdateAsync(updateDto);
            return Ok(investigacion);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Elimina una investigación (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var exists = await _investigacionService.ExistsAsync(id);
            if (!exists)
                return NotFound(new { message = "Investigación no encontrada" });

            var deleted = await _investigacionService.DeleteAsync(id);
            if (!deleted)
                return BadRequest(new { message = "No se pudo eliminar la investigación" });

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }
}
