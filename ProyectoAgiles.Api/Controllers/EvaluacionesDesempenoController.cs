using Microsoft.AspNetCore.Mvc;
using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Application.Interfaces;

namespace ProyectoAgiles.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EvaluacionesDesempenoController : ControllerBase
{
    private readonly IEvaluacionDesempenoService _evaluacionService;

    public EvaluacionesDesempenoController(IEvaluacionDesempenoService evaluacionService)
    {
        _evaluacionService = evaluacionService;
    }

    /// <summary>
    /// Obtiene todas las evaluaciones de desempeño
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EvaluacionDesempenoDto>>> GetAll()
    {
        try
        {
            var evaluaciones = await _evaluacionService.GetAllAsync();
            return Ok(evaluaciones);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene una evaluación por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<EvaluacionDesempenoDto>> GetById(int id)
    {
        try
        {
            var evaluacion = await _evaluacionService.GetByIdAsync(id);
            if (evaluacion == null)
                return NotFound(new { message = "Evaluación no encontrada" });

            return Ok(evaluacion);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene evaluaciones por cédula del docente
    /// </summary>
    [HttpGet("by-cedula/{cedula}")]
    public async Task<ActionResult<IEnumerable<EvaluacionDesempenoDto>>> GetByCedula(string cedula)
    {
        try
        {
            var evaluaciones = await _evaluacionService.GetByCedulaAsync(cedula);
            return Ok(evaluaciones);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene las últimas 4 evaluaciones de un docente
    /// </summary>
    [HttpGet("by-cedula/{cedula}/ultimas-cuatro")]
    public async Task<ActionResult<IEnumerable<EvaluacionDesempenoDto>>> GetUltimasCuatroEvaluacionesByCedula(string cedula)
    {
        try
        {
            var evaluaciones = await _evaluacionService.GetUltimasCuatroEvaluacionesByCedulaAsync(cedula);
            return Ok(evaluaciones);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene evaluaciones por período académico
    /// </summary>
    [HttpGet("by-periodo/{periodoAcademico}")]
    public async Task<ActionResult<IEnumerable<EvaluacionDesempenoDto>>> GetByPeriodoAcademico(string periodoAcademico)
    {
        try
        {
            var evaluaciones = await _evaluacionService.GetByPeriodoAcademicoAsync(periodoAcademico);
            return Ok(evaluaciones);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene evaluaciones por año
    /// </summary>
    [HttpGet("by-anio/{anio}")]
    public async Task<ActionResult<IEnumerable<EvaluacionDesempenoDto>>> GetByAnio(int anio)
    {
        try
        {
            var evaluaciones = await _evaluacionService.GetByAnioAsync(anio);
            return Ok(evaluaciones);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene evaluaciones por año y semestre
    /// </summary>
    [HttpGet("by-anio/{anio}/semestre/{semestre}")]
    public async Task<ActionResult<IEnumerable<EvaluacionDesempenoDto>>> GetByAnioAndSemestre(int anio, int semestre)
    {
        try
        {
            if (semestre < 1 || semestre > 2)
                return BadRequest(new { message = "El semestre debe ser 1 o 2" });

            var evaluaciones = await _evaluacionService.GetByAnioAndSemestreAsync(anio, semestre);
            return Ok(evaluaciones);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Crea una nueva evaluación de desempeño
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<EvaluacionDesempenoDto>> Create([FromBody] CreateEvaluacionDesempenoDto createDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var evaluacion = await _evaluacionService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = evaluacion.Id }, evaluacion);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Crea una nueva evaluación con archivo PDF
    /// </summary>
    [HttpPost("con-pdf")]
    public async Task<IActionResult> CreateWithPdf([FromForm] CreateEvaluacionWithPdfDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var evaluacion = await _evaluacionService.CreateWithPdfAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = evaluacion.Id }, evaluacion);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza una evaluación existente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<EvaluacionDesempenoDto>> Update(int id, [FromBody] UpdateEvaluacionDesempenoDto updateDto)
    {
        try
        {
            if (id != updateDto.Id)
                return BadRequest(new { message = "El ID de la URL no coincide con el ID del objeto" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var exists = await _evaluacionService.ExistsAsync(id);
            if (!exists)
                return NotFound(new { message = "Evaluación no encontrada" });

            var evaluacion = await _evaluacionService.UpdateAsync(updateDto);
            return Ok(evaluacion);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Elimina una evaluación (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var exists = await _evaluacionService.ExistsAsync(id);
            if (!exists)
                return NotFound(new { message = "Evaluación no encontrada" });

            var deleted = await _evaluacionService.DeleteAsync(id);
            if (!deleted)
                return BadRequest(new { message = "No se pudo eliminar la evaluación" });

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene el resumen de evaluaciones de un docente
    /// </summary>
    [HttpGet("resumen/{cedula}")]
    public async Task<ActionResult<ResumenEvaluacionesDto>> GetResumenEvaluaciones(string cedula)
    {
        try
        {
            var resumen = await _evaluacionService.GetResumenEvaluacionesByCedulaAsync(cedula);
            return Ok(resumen);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Verifica si un docente cumple con el requisito del 75%
    /// </summary>
    [HttpGet("verificar-requisito-75/{cedula}")]
    public async Task<ActionResult<VerificacionRequisito75Dto>> VerificarRequisito75PorCiento(string cedula)
    {
        try
        {
            var verificacion = await _evaluacionService.VerificarRequisito75PorCientoAsync(cedula);
            return Ok(verificacion);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene evaluaciones que alcanzan el 75%
    /// </summary>
    [HttpGet("que-alcanzan-75")]
    public async Task<ActionResult<IEnumerable<EvaluacionDesempenoDto>>> GetEvaluacionesQueAlcanzan75PorCiento()
    {
        try
        {
            var evaluaciones = await _evaluacionService.GetEvaluacionesQueAlcanzan75PorCientoAsync();
            return Ok(evaluaciones);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene evaluaciones que alcanzan el 75% por cédula
    /// </summary>
    [HttpGet("que-alcanzan-75/{cedula}")]
    public async Task<ActionResult<IEnumerable<EvaluacionDesempenoDto>>> GetEvaluacionesQueAlcanzan75PorCientoByCedula(string cedula)
    {
        try
        {
            var evaluaciones = await _evaluacionService.GetEvaluacionesQueAlcanzan75PorCientoByCedulaAsync(cedula);
            return Ok(evaluaciones);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene el PDF de una evaluación
    /// </summary>
    [HttpGet("{id}/pdf")]
    public async Task<IActionResult> GetPdf(int id)
    {
        try
        {
            var pdfBytes = await _evaluacionService.GetPdfByIdAsync(id);
            
            if (pdfBytes == null || pdfBytes.Length == 0)
                return NotFound(new { message = "PDF no encontrado para esta evaluación" });

            return File(pdfBytes, "application/pdf", $"evaluacion_desempeno_{id}.pdf");
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene estadísticas generales de evaluaciones
    /// </summary>
    [HttpGet("estadisticas")]
    public async Task<ActionResult> GetEstadisticasGenerales()
    {
        try
        {
            var estadisticas = await _evaluacionService.GetEstadisticasGeneralesAsync();
            return Ok(estadisticas);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Verifica si existe evaluación para un período específico
    /// </summary>
    [HttpGet("existe/{cedula}/{periodoAcademico}")]
    public async Task<ActionResult<bool>> ExisteEvaluacionParaPeriodo(string cedula, string periodoAcademico)
    {
        try
        {
            var existe = await _evaluacionService.ExisteEvaluacionParaPeriodoAsync(cedula, periodoAcademico);
            return Ok(new { existe });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// Inserta datos de prueba para evaluaciones (solo en desarrollo)
    /// </summary>
    [HttpPost("seed-test-data")]
    public async Task<ActionResult> SeedTestData()
    {
        try
        {
            var testData = new List<CreateEvaluacionDesempenoDto>
            {
                new CreateEvaluacionDesempenoDto
                {
                    Cedula = "1805123456",
                    PeriodoAcademico = "2024-1",
                    Anio = 2024,
                    Semestre = 1,
                    PuntajeObtenido = 85.5m,
                    PuntajeMaximo = 100,
                    FechaEvaluacion = new DateTime(2024, 6, 15),
                    TipoEvaluacion = "Integral",
                    Estado = "Completada",
                    Evaluador = "Comisión Evaluadora",
                    Observaciones = "Excelente desempeño en docencia e investigación"
                },
                new CreateEvaluacionDesempenoDto
                {
                    Cedula = "1805123456",
                    PeriodoAcademico = "2023-2",
                    Anio = 2023,
                    Semestre = 2,
                    PuntajeObtenido = 78.2m,
                    PuntajeMaximo = 100,
                    FechaEvaluacion = new DateTime(2024, 1, 20),
                    TipoEvaluacion = "Integral",
                    Estado = "Completada",
                    Evaluador = "Comisión Evaluadora",
                    Observaciones = "Buen desempeño, cumple con estándares requeridos"
                },
                new CreateEvaluacionDesempenoDto
                {
                    Cedula = "1805123456",
                    PeriodoAcademico = "2023-1",
                    Anio = 2023,
                    Semestre = 1,
                    PuntajeObtenido = 82.8m,
                    PuntajeMaximo = 100,
                    FechaEvaluacion = new DateTime(2023, 6, 25),
                    TipoEvaluacion = "Integral",
                    Estado = "Completada",
                    Evaluador = "Comisión Evaluadora",
                    Observaciones = "Muy buen desempeño académico y en gestión"
                },
                new CreateEvaluacionDesempenoDto
                {
                    Cedula = "1805123456",
                    PeriodoAcademico = "2022-2",
                    Anio = 2022,
                    Semestre = 2,
                    PuntajeObtenido = 76.5m,
                    PuntajeMaximo = 100,
                    FechaEvaluacion = new DateTime(2023, 1, 15),
                    TipoEvaluacion = "Integral",
                    Estado = "Completada",
                    Evaluador = "Comisión Evaluadora",
                    Observaciones = "Cumple con los requisitos mínimos establecidos"
                }
            };

            var createdEvaluaciones = new List<EvaluacionDesempenoDto>();
            foreach (var data in testData)
            {
                try
                {
                    var created = await _evaluacionService.CreateAsync(data);
                    createdEvaluaciones.Add(created);
                }
                catch (InvalidOperationException)
                {
                    // Si ya existe, continúa con el siguiente
                    continue;
                }
            }

            return Ok(new { 
                message = "Datos de prueba insertados exitosamente", 
                count = createdEvaluaciones.Count,
                evaluaciones = createdEvaluaciones 
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al insertar datos de prueba", error = ex.Message });
        }
    }
}
