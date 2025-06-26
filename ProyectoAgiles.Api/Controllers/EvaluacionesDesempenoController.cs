using Microsoft.AspNetCore.Mvc;
using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Application.Interfaces;
using ProyectoAgiles.Domain.Interfaces;
using System.Linq;
using Swashbuckle.AspNetCore.Annotations;

namespace ProyectoAgiles.Api.Controllers;

/// <summary>
/// Controlador de Evaluaciones de Desempeño Docente
/// </summary>
/// <remarks>
/// Este controlador maneja todas las operaciones relacionadas con las evaluaciones de desempeño académico,
/// incluyendo registro, consulta, análisis y generación de estadísticas para el escalafón docente.
/// 
/// <para>
/// <strong>Funcionalidades principales:</strong>
/// </para>
/// <list type="bullet">
/// <item><description>Registro y gestión de evaluaciones</description></item>
/// <item><description>Análisis de rendimiento académico</description></item>
/// <item><description>Estadísticas y reportes</description></item>
/// <item><description>Verificación de requisitos de promoción</description></item>
/// <item><description>Gestión de documentos de respaldo</description></item>
/// </list>
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("Evaluaciones de Desempeño")]
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
    /// Obtener todas las evaluaciones de desempeño
    /// </summary>
    /// <remarks>
    /// Recupera una lista completa de todas las evaluaciones de desempeño registradas en el sistema.
    /// 
    /// <para><strong>Información incluida por cada evaluación:</strong></para>
    /// <list type="bullet">
    /// <item><description>Datos del docente evaluado</description></item>
    /// <item><description>Período académico y fechas</description></item>
    /// <item><description>Puntajes obtenidos y máximos</description></item>
    /// <item><description>Observaciones y comentarios</description></item>
    /// <item><description>Estado de la evaluación</description></item>
    /// <item><description>Evaluador responsable</description></item>
    /// </list>
    /// 
    /// <para><strong>Casos de uso:</strong></para>
    /// <list type="bullet">
    /// <item><description>Dashboard administrativo</description></item>
    /// <item><description>Análisis de rendimiento general</description></item>
    /// <item><description>Reportes institucionales</description></item>
    /// </list>
    /// </remarks>
    /// <returns>Lista completa de evaluaciones de desempeño</returns>
    /// <response code="200">Lista de evaluaciones obtenida exitosamente</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EvaluacionDesempenoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Obtener todas las evaluaciones de desempeño",
        Description = "Recupera la lista completa de evaluaciones de desempeño del sistema",
        OperationId = "GetAllEvaluaciones",
        Tags = new[] { "Evaluaciones de Desempeño" }
    )]
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
    /// Obtener evaluación específica por ID
    /// </summary>
    /// <remarks>
    /// Recupera información detallada de una evaluación de desempeño específica mediante su identificador único.
    /// 
    /// <para><strong>Información detallada incluida:</strong></para>
    /// <list type="bullet">
    /// <item><description>Datos completos de la evaluación</description></item>
    /// <item><description>Información del docente evaluado</description></item>
    /// <item><description>Desglose detallado de puntajes</description></item>
    /// <item><description>Documentos de respaldo asociados</description></item>
    /// <item><description>Observaciones y recomendaciones</description></item>
    /// <item><description>Historial de cambios</description></item>
    /// </list>
    /// 
    /// <para><strong>Uso típico:</strong> Vista detallada, edición, análisis individual</para>
    /// </remarks>
    /// <param name="id">Identificador único de la evaluación</param>
    /// <returns>Información completa de la evaluación</returns>
    /// <response code="200">Evaluación encontrada exitosamente</response>
    /// <response code="404">Evaluación no encontrada</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EvaluacionDesempenoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Obtener evaluación por ID",
        Description = "Recupera la información detallada de una evaluación de desempeño específica",
        OperationId = "GetEvaluacionById",
        Tags = new[] { "Evaluaciones de Desempeño" }
    )]
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
    /// 🔍 Buscar evaluaciones por cédula del docente
    /// </summary>
    /// <remarks>
    /// Recupera todas las evaluaciones de desempeño asociadas a un docente específico identificado por su cédula.
    /// 
    /// <para><strong>Información proporcionada:</strong></para>
    /// <list type="bullet">
    /// <item><description>Historial completo de evaluaciones</description></item>
    /// <item><description>Evaluaciones ordenadas por fecha</description></item>
    /// <item><description>Evolución del rendimiento</description></item>
    /// <item><description>Tendencias de mejora o deterioro</description></item>
    /// </list>
    /// 
    /// <para><strong>Casos de uso:</strong></para>
    /// <list type="bullet">
    /// <item><description>Perfil académico del docente</description></item>
    /// <item><description>Análisis de progreso personal</description></item>
    /// <item><description>Evaluación para promociones</description></item>
    /// <item><description>Reportes individuales</description></item>
    /// </list>
    /// 
    /// <para><strong>Ejemplo de búsqueda:</strong> <c>GET /api/EvaluacionesDesempeno/by-cedula/1234567890</c></para>
    /// </remarks>
    /// <param name="cedula">Número de cédula del docente</param>
    /// <returns>Lista de evaluaciones del docente especificado</returns>
    /// <response code="200">Evaluaciones encontradas exitosamente</response>
    /// <response code="400">Cédula con formato inválido</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("by-cedula/{cedula}")]
    [ProducesResponseType(typeof(IEnumerable<EvaluacionDesempenoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Buscar evaluaciones por cédula",
        Description = "Recupera todas las evaluaciones de desempeño de un docente específico",
        OperationId = "GetEvaluacionesByCedula",
        Tags = new[] { "Búsquedas" }
    )]
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
    /// Obtener las últimas 4 evaluaciones de un docente
    /// </summary>
    /// <remarks>
    /// Recupera las últimas cuatro evaluaciones de desempeño de un docente, utilizadas para calcular el promedio requerido para promociones.
    /// 
    /// <para><strong>Criterios de selección:</strong></para>
    /// <list type="bullet">
    /// <item><description>Evaluaciones más recientes por fecha</description></item>
    /// <item><description>Solo evaluaciones completadas</description></item>
    /// <item><description>Ordenadas cronológicamente</description></item>
    /// </list>
    /// 
    /// <para><strong>Uso principal:</strong> Cálculo del requisito del 75% para promociones docentes</para>
    /// </remarks>
    /// <param name="cedula">Número de cédula del docente</param>
    /// <returns>Lista de las últimas 4 evaluaciones del docente</returns>
    /// <response code="200">Evaluaciones obtenidas exitosamente</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("by-cedula/{cedula}/ultimas-cuatro")]
    [ProducesResponseType(typeof(IEnumerable<EvaluacionDesempenoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Obtener últimas 4 evaluaciones",
        Description = "Recupera las últimas cuatro evaluaciones de un docente para análisis de promoción",
        OperationId = "GetUltimasCuatroEvaluaciones",
        Tags = new[] { "Búsquedas" }
    )]
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
    /// Filtrar evaluaciones por período académico
    /// </summary>
    /// <remarks>
    /// Recupera todas las evaluaciones realizadas en un período académico específico.
    /// 
    /// <para><strong>Utilidad del filtrado:</strong></para>
    /// <list type="bullet">
    /// <item><description>Análisis por semestres</description></item>
    /// <item><description>Comparación entre períodos</description></item>
    /// <item><description>Reportes administrativos</description></item>
    /// </list>
    /// </remarks>
    /// <param name="periodoAcademico">Período académico (ej: 2024-1, 2024-2)</param>
    /// <returns>Lista de evaluaciones del período especificado</returns>
    /// <response code="200">Evaluaciones del período obtenidas</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("by-periodo/{periodoAcademico}")]
    [ProducesResponseType(typeof(IEnumerable<EvaluacionDesempenoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Filtrar por período académico",
        Description = "Recupera evaluaciones de un período académico específico",
        OperationId = "GetEvaluacionesByPeriodo",
        Tags = new[] { "Búsquedas" }
    )]
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
    /// Crear nueva evaluación de desempeño
    /// </summary>
    /// <remarks>
    /// Registra una nueva evaluación de desempeño docente en el sistema con validación completa.
    /// 
    /// <para><strong>Datos requeridos:</strong></para>
    /// <list type="bullet">
    /// <item><description>Cédula del docente evaluado</description></item>
    /// <item><description>Período académico y fechas</description></item>
    /// <item><description>Puntajes obtenidos y máximos</description></item>
    /// <item><description>Evaluador responsable</description></item>
    /// <item><description>Observaciones (opcional)</description></item>
    /// </list>
    /// 
    /// <para><strong>Validaciones aplicadas:</strong></para>
    /// <list type="bullet">
    /// <item><description>Docente debe existir en el sistema</description></item>
    /// <item><description>No duplicar evaluaciones por período</description></item>
    /// <item><description>Puntajes dentro de rangos válidos</description></item>
    /// <item><description>Campos obligatorios completos</description></item>
    /// </list>
    /// </remarks>
    /// <param name="createDto">Datos para crear la nueva evaluación</param>
    /// <returns>Evaluación creada con su ID asignado</returns>
    /// <response code="201">Evaluación creada exitosamente</response>
    /// <response code="400">Datos inválidos o incompletos</response>
    /// <response code="409">Ya existe evaluación para este período</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost]
    [ProducesResponseType(typeof(EvaluacionDesempenoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Crear nueva evaluación de desempeño",
        Description = "Registra una nueva evaluación de desempeño docente con validación completa",
        OperationId = "CreateEvaluacion",
        Tags = new[] { "Evaluaciones de Desempeño" }
    )]
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
    /// Verificar requisito del 75% para promoción
    /// </summary>
    /// <remarks>
    /// Verifica si un docente cumple con el requisito del 75% de promedio en las últimas 4 evaluaciones para optar a promoción.
    /// 
    /// <para><strong>Criterios de evaluación:</strong></para>
    /// <list type="bullet">
    /// <item><description>Promedio de últimas 4 evaluaciones ≥ 75%</description></item>
    /// <item><description>Evaluaciones deben estar completadas</description></item>
    /// <item><description>Consideración de períodos consecutivos</description></item>
    /// </list>
    /// 
    /// <para><strong>Información de respuesta:</strong></para>
    /// <list type="bullet">
    /// <item><description>Promedio calculado</description></item>
    /// <item><description>Estado de cumplimiento</description></item>
    /// <item><description>Desglose por evaluación</description></item>
    /// <item><description>Mensaje explicativo</description></item>
    /// </list>
    /// 
    /// <para><strong>Uso crítico:</strong> Proceso de promoción y escalafón docente</para>
    /// </remarks>
    /// <param name="cedula">Número de cédula del docente a evaluar</param>
    /// <returns>Resultado de la verificación del requisito del 75%</returns>
    /// <response code="200">Verificación completada exitosamente</response>
    /// <response code="400">Cédula inválida</response>
    /// <response code="404">Docente no encontrado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("verificar-requisito-75/{cedula}")]
    [ProducesResponseType(typeof(VerificacionRequisito75Dto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Verificar requisito del 75%",
        Description = "Verifica si un docente cumple el requisito del 75% de promedio para promoción",
        OperationId = "VerificarRequisito75",
        Tags = new[] { "Análisis de Promoción" }
    )]
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
    /// Descargar documento PDF de evaluación
    /// </summary>
    /// <remarks>
    /// Descarga el archivo PDF de respaldo asociado a una evaluación de desempeño específica.
    /// 
    /// <para><strong>Características de la descarga:</strong></para>
    /// <list type="bullet">
    /// <item><description>Formato PDF nativo</description></item>
    /// <item><description>Descarga segura y validada</description></item>
    /// <item><description>Nombre descriptivo del archivo</description></item>
    /// <item><description>Optimizado para archivos grandes</description></item>
    /// </list>
    /// 
    /// <para><strong>Casos de uso:</strong></para>
    /// <list type="bullet">
    /// <item><description>Revisión de documentos de respaldo</description></item>
    /// <item><description>Auditoría de evaluaciones</description></item>
    /// <item><description>Generación de reportes oficiales</description></item>
    /// </list>
    /// </remarks>
    /// <param name="id">ID de la evaluación cuyo PDF se desea descargar</param>
    /// <returns>Archivo PDF para descarga directa</returns>
    /// <response code="200">PDF descargado exitosamente</response>
    /// <response code="404">Evaluación o PDF no encontrado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("{id}/pdf")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Descargar PDF de evaluación",
        Description = "Descarga el documento PDF de respaldo de una evaluación específica",
        OperationId = "GetEvaluacionPdf",
        Tags = new[] { "Archivos" }
    )]
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
    /// Obtener estadísticas completas de promoción docente
    /// </summary>
    /// <remarks>
    /// Genera un reporte completo del estado de un docente respecto a todos los requisitos para promoción en el escalafón.
    /// 
    /// <para><strong>Requisitos evaluados:</strong></para>
    /// <list type="bullet">
    /// <item><description>Experiencia: 4 años como titular auxiliar</description></item>
    /// <item><description>Obras: Al menos una publicación con filiación UTA</description></item>
    /// <item><description>Evaluaciones: Promedio ≥75% en últimas 4 evaluaciones</description></item>
    /// <item><description>Capacitaciones: 96 horas profesionales + 24 pedagógicas</description></item>
    /// </list>
    /// 
    /// <para><strong>Información detallada por sección:</strong></para>
    /// <list type="bullet">
    /// <item><description>Estado de cumplimiento individual</description></item>
    /// <item><description>Porcentaje de completitud</description></item>
    /// <item><description>Estadísticas específicas</description></item>
    /// <item><description>Recomendaciones de mejora</description></item>
    /// </list>
    /// 
    /// <para><strong>Resultado final:</strong></para>
    /// <list type="bullet">
    /// <item><description>Porcentaje general de completitud</description></item>
    /// <item><description>Elegibilidad para promoción</description></item>
    /// <item><description>Resumen ejecutivo</description></item>
    /// </list>
    /// 
    /// <para><strong>Uso crítico:</strong> Decisiones de promoción en escalafón docente UTA</para>
    /// </remarks>
    /// <param name="cedula">Número de cédula del docente a evaluar</param>
    /// <returns>Estadísticas completas para promoción docente</returns>
    /// <response code="200">Estadísticas generadas exitosamente</response>
    /// <response code="400">Cédula inválida</response>
    /// <response code="404">Docente no encontrado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("estadisticas-docente/{cedula}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Estadísticas completas de promoción",
        Description = "Genera reporte completo del estado de un docente para promoción en escalafón",
        OperationId = "GetEstadisticasPromocion",
        Tags = new[] { "Análisis de Promoción" }
    )]
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
