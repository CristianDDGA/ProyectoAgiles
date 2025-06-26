using Microsoft.AspNetCore.Mvc;
using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Application.Interfaces;
using ProyectoAgiles.Domain.Interfaces;
using System.Linq;

namespace ProyectoAgiles.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EvaluacionesDesempenoController : ControllerBase
{
    private readonly IEvaluacionDesempenoService _evaluacionService;
    private readonly ITTHHRepository _tthhRepository;
    private readonly IInvestigacionService _investigacionService;
    private readonly IDiticService _diticService;

    public EvaluacionesDesempenoController(
        IEvaluacionDesempenoService evaluacionService,
        ITTHHRepository tthhRepository,
        IInvestigacionService investigacionService,
        IDiticService diticService)
    {
        _evaluacionService = evaluacionService;
        _tthhRepository = tthhRepository;
        _investigacionService = investigacionService;
        _diticService = diticService;
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
    /// Actualiza una evaluación existente con archivo PDF
    /// </summary>
    [HttpPut("{id}/con-pdf")]
    public async Task<ActionResult<EvaluacionDesempenoDto>> UpdateWithPdf(int id, [FromForm] UpdateEvaluacionWithPdfDto dto)
    {
        try
        {
            if (id != dto.Id)
                return BadRequest(new { message = "El ID de la URL no coincide con el ID del objeto" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var exists = await _evaluacionService.ExistsAsync(id);
            if (!exists)
                return NotFound(new { message = "Evaluación no encontrada" });

            var evaluacion = await _evaluacionService.UpdateWithPdfAsync(dto);
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
    /// Obtiene estadísticas completas de un docente para los requisitos de promoción
    /// </summary>
    [HttpGet("estadisticas-docente/{cedula}")]
    public async Task<ActionResult> GetEstadisticasDocente(string cedula)
    {
        try
        {
            // 1. Experiencia (años de servicio desde TTHH)
            var experienciaStats = new
            {
                titulo = "Experiencia Académica",
                icono = "fas fa-clock",
                color = "primary",
                datos = new
                {
                    añosRequeridos = 4,
                    añosObtenidos = 0.0,
                    cumple = false,
                    detalles = "Años como titular auxiliar 1"
                }
            };

            try
            {
                var tthhData = await _tthhRepository.GetByCedulaAsync(cedula);
                if (tthhData != null)
                {
                    var fechaIngreso = tthhData.FechaInicio;
                    var añosExperiencia = Math.Round((DateTime.Now - fechaIngreso).TotalDays / 365.25, 1);
                    
                    experienciaStats = new
                    {
                        titulo = "Experiencia Académica",
                        icono = "fas fa-clock",
                        color = "primary",
                        datos = new
                        {
                            añosRequeridos = 4,
                            añosObtenidos = añosExperiencia,
                            cumple = añosExperiencia >= 4,
                            detalles = $"Años de servicio desde {fechaIngreso:yyyy-MM-dd}"
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                experienciaStats = new
                {
                    titulo = "Experiencia Académica",
                    icono = "fas fa-clock",
                    color = "primary",
                    datos = new
                    {
                        añosRequeridos = 4,
                        añosObtenidos = 0.0,
                        cumple = false,
                        detalles = $"Error al obtener datos: {ex.Message}"
                    }
                };
            }            // 2. Obras/Investigaciones
            var obrasStats = new
            {
                titulo = "Obras e Investigaciones",
                icono = "fas fa-book",
                color = "success",
                datos = new
                {
                    totalObras = 0,
                    obrasConUTA = 0,
                    cumple = false,
                    mensaje = "NO CUMPLE",
                    detalles = "Obras relevantes con filiación UTA",
                    estadisticas = new
                    {
                        investigacionesAnalizadas = 0,
                        conFiliacionUTA = 0,
                        porcentajeUTA = 0.0
                    }
                }
            };try
            {
                // Usar el mismo endpoint que usa AuthService para garantizar consistencia
                using var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(Request.Scheme + "://" + Request.Host);
                var investigacionesResponse = await httpClient.GetFromJsonAsync<List<InvestigacionDto>>($"/api/investigaciones/by-cedula/{cedula}");
                var investigaciones = investigacionesResponse ?? new List<InvestigacionDto>();
                
                // Logging para debug
                Console.WriteLine($"DEBUG: Total investigaciones encontradas: {investigaciones.Count}");
                foreach (var inv in investigaciones)
                {
                    Console.WriteLine($"DEBUG: Investigación: {inv.Titulo} - Filiación: '{inv.Filiacion}'");
                }
                
                // Usar exactamente la misma lógica que AuthService
                var obrasConUTA = investigaciones.Count(i => 
                    !string.IsNullOrWhiteSpace(i.Filiacion) && (
                        i.Filiacion.Contains("UTA", StringComparison.OrdinalIgnoreCase) ||
                        i.Filiacion.Contains("Universidad Técnica de Ambato", StringComparison.OrdinalIgnoreCase)
                    ));

                Console.WriteLine($"DEBUG: Obras con UTA detectadas: {obrasConUTA}");
                  obrasStats = new
                {
                    titulo = "Obras e Investigaciones",
                    icono = "fas fa-book",
                    color = "success",
                    datos = new
                    {
                        totalObras = investigaciones.Count,
                        obrasConUTA = obrasConUTA,
                        cumple = obrasConUTA > 0,
                        mensaje = obrasConUTA > 0 ? "CUMPLE" : "NO CUMPLE",
                        detalles = $"El docente {(obrasConUTA > 0 ? "cumple" : "no cumple")} con el requisito de obra relevante con filiación UTA.",
                        estadisticas = new
                        {
                            investigacionesAnalizadas = investigaciones.Count,
                            conFiliacionUTA = obrasConUTA,
                            porcentajeUTA = investigaciones.Count > 0 ? Math.Round((double)obrasConUTA / investigaciones.Count * 100, 1) : 0
                        }
                    }
                };
            }            catch (Exception ex)
            {
                obrasStats = new
                {
                    titulo = "Obras e Investigaciones",
                    icono = "fas fa-book",
                    color = "success",
                    datos = new
                    {
                        totalObras = 0,
                        obrasConUTA = 0,
                        cumple = false,
                        mensaje = "ERROR",
                        detalles = $"Error al obtener datos: {ex.Message}",
                        estadisticas = new
                        {
                            investigacionesAnalizadas = 0,
                            conFiliacionUTA = 0,
                            porcentajeUTA = 0.0
                        }
                    }
                };
            }

            // 3. Evaluaciones DAC
            var evaluacionesStats = new
            {
                titulo = "Evaluaciones de Desempeño",
                icono = "fas fa-star",
                color = "warning",
                datos = new
                {
                    evaluacionesAnalizadas = 0,
                    promedioObtenido = 0.0m,
                    requiere75 = 75.0m,
                    cumple = false,
                    detalles = "Promedio últimas 4 evaluaciones"
                }
            };

            try
            {
                var verificacionEvaluaciones = await _evaluacionService.VerificarRequisito75PorCientoAsync(cedula);
                evaluacionesStats = new
                {
                    titulo = "Evaluaciones de Desempeño",
                    icono = "fas fa-star",
                    color = "warning",
                    datos = new
                    {
                        evaluacionesAnalizadas = verificacionEvaluaciones.EvaluacionesAnalizadas,
                        promedioObtenido = verificacionEvaluaciones.PorcentajePromedioUltimasCuatro,
                        requiere75 = 75.0m,
                        cumple = verificacionEvaluaciones.CumpleRequisito,
                        detalles = verificacionEvaluaciones.Mensaje
                    }
                };
            }
            catch (Exception ex)
            {
                evaluacionesStats = new
                {
                    titulo = "Evaluaciones de Desempeño",
                    icono = "fas fa-star",
                    color = "warning",
                    datos = new
                    {
                        evaluacionesAnalizadas = 0,
                        promedioObtenido = 0.0m,
                        requiere75 = 75.0m,
                        cumple = false,
                        detalles = $"Error al obtener datos: {ex.Message}"
                    }
                };
            }            // 4. Capacitaciones DITIC
            var capacitacionStats = new
            {
                titulo = "Capacitaciones Profesionales",
                icono = "fas fa-graduation-cap",
                color = "info",
                datos = new
                {
                    horasRequeridas = 96,
                    horasObtenidas = 0,
                    horasPedagogicasRequeridas = 24,
                    horasPedagogicasObtenidas = 0,
                    cumple = false,
                    mensaje = "NO CUMPLE",
                    detalles = "Capacitaciones últimos 3 años",
                    estadisticas = new
                    {
                        capacitacionesAnalizadas = 0,
                        horasAcumuladas = 0,
                        horasPedagogicasAcumuladas = 0,
                        porcentajeCompletitud = 0.0
                    }
                }
            };            try
            {
                var verificacionCapacitacion = await _diticService.VerifyRequirementAsync(cedula);
                var porcentajeCompletitudCapacitacion = verificacionCapacitacion.HorasObtenidas > 0 ? Math.Round((double)verificacionCapacitacion.HorasObtenidas / 96 * 100, 1) : 0;
                
                capacitacionStats = new
                {
                    titulo = "Capacitaciones Profesionales",
                    icono = "fas fa-graduation-cap",
                    color = "info",
                    datos = new
                    {
                        horasRequeridas = 96,
                        horasObtenidas = verificacionCapacitacion.HorasObtenidas,
                        horasPedagogicasRequeridas = 24,
                        horasPedagogicasObtenidas = verificacionCapacitacion.HorasPedagogicasObtenidas,
                        cumple = verificacionCapacitacion.CumpleRequisito,
                        mensaje = verificacionCapacitacion.CumpleRequisito ? "CUMPLE" : "NO CUMPLE",
                        detalles = $"El docente {(verificacionCapacitacion.CumpleRequisito ? "cumple" : "no cumple")} con el requisito de 96 horas de capacitación.",
                        estadisticas = new
                        {
                            capacitacionesAnalizadas = verificacionCapacitacion.CapacitacionesAnalizadas,
                            horasAcumuladas = verificacionCapacitacion.HorasObtenidas,
                            horasPedagogicasAcumuladas = verificacionCapacitacion.HorasPedagogicasObtenidas,
                            porcentajeCompletitud = porcentajeCompletitudCapacitacion
                        }
                    }
                };
            }            catch (Exception ex)
            {
                capacitacionStats = new
                {
                    titulo = "Capacitaciones Profesionales",
                    icono = "fas fa-graduation-cap",
                    color = "info",
                    datos = new
                    {
                        horasRequeridas = 96,
                        horasObtenidas = 0,
                        horasPedagogicasRequeridas = 24,
                        horasPedagogicasObtenidas = 0,
                        cumple = false,
                        mensaje = "ERROR",
                        detalles = $"Error al obtener datos: {ex.Message}",
                        estadisticas = new
                        {
                            capacitacionesAnalizadas = 0,
                            horasAcumuladas = 0,
                            horasPedagogicasAcumuladas = 0,
                            porcentajeCompletitud = 0.0
                        }
                    }
                };
            }

            // Calcular requisitos cumplidos
            var requisitosCumplidos = 0;
            if (experienciaStats.datos.cumple) requisitosCumplidos++;
            if (obrasStats.datos.cumple) requisitosCumplidos++;
            if (evaluacionesStats.datos.cumple) requisitosCumplidos++;
            if (capacitacionStats.datos.cumple) requisitosCumplidos++;

            var porcentajeCompletitud = Math.Round((double)requisitosCumplidos / 4 * 100, 1);

            var resultado = new
            {
                cedula = cedula,
                fechaConsulta = DateTime.Now,
                resumen = new
                {
                    totalRequisitos = 4,
                    requisitosCumplidos = requisitosCumplidos,
                    porcentajeCompletitud = porcentajeCompletitud,
                    puedeSubirNivel = requisitosCumplidos == 4
                },
                secciones = new
                {
                    experiencia = experienciaStats,
                    obras = obrasStats,
                    evaluaciones = evaluacionesStats,
                    capacitaciones = capacitacionStats
                }
            };

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener estadísticas del docente", error = ex.Message });
        }
    }
}
