using Microsoft.AspNetCore.Mvc;
using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace ProyectoAgiles.Api.Controllers;

/// <summary>
/// 🔬 Controlador de Gestión de Investigaciones
/// </summary>
/// <remarks>
/// Este controlador maneja todas las operaciones relacionadas con la gestión de investigaciones académicas,
/// incluyendo registro, seguimiento, evaluación y administración de proyectos de investigación.
/// 
/// <para>
/// <strong>Funcionalidades principales:</strong>
/// </para>
/// <list type="bullet">
/// <item><description>📝 Registro y gestión de proyectos de investigación</description></item>
/// <item><description>👥 Asignación de investigadores y colaboradores</description></item>
/// <item><description>📊 Seguimiento de progreso y resultados</description></item>
/// <item><description>📋 Evaluación y validación de investigaciones</description></item>
/// <item><description>📈 Reportes y estadísticas de investigación</description></item>
/// </list>
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("🔬 Investigaciones")]
public class InvestigacionesController : ControllerBase
{
    private readonly IInvestigacionService _investigacionService;

    public InvestigacionesController(IInvestigacionService investigacionService)
    {
        _investigacionService = investigacionService;
    }

    /// <summary>
    /// 📋 Obtener todas las investigaciones del sistema
    /// </summary>
    /// <remarks>
    /// Recupera una lista completa de todas las investigaciones registradas en el sistema.
    /// 
    /// <para><strong>Información incluida por cada investigación:</strong></para>
    /// <list type="bullet">
    /// <item><description>📝 Título y descripción del proyecto</description></item>
    /// <item><description>👤 Investigador principal y colaboradores</description></item>
    /// <item><description>📅 Fechas de inicio y finalización</description></item>
    /// <item><description>💰 Presupuesto y financiamiento</description></item>
    /// <item><description>🎯 Estado actual del proyecto</description></item>
    /// <item><description>📊 Resultados y publicaciones</description></item>
    /// </list>
    /// 
    /// <para><strong>💡 Casos de uso:</strong></para>
    /// <list type="bullet">
    /// <item><description>📊 Dashboard de investigaciones</description></item>
    /// <item><description>📈 Reportes administrativos</description></item>
    /// <item><description>🔍 Búsqueda y filtrado</description></item>
    /// </list>
    /// </remarks>
    /// <returns>Lista completa de investigaciones del sistema</returns>
    /// <response code="200">✅ Lista de investigaciones obtenida exitosamente</response>
    /// <response code="500">💥 Error interno del servidor</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<InvestigacionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Obtener todas las investigaciones",
        Description = "Recupera una colección completa de todas las investigaciones registradas en el sistema",
        OperationId = "GetAllInvestigaciones",
        Tags = new[] { "🔬 Investigaciones" }
    )]
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
    /// 🔍 Obtener investigación específica por ID
    /// </summary>
    /// <remarks>
    /// Recupera información detallada de una investigación específica mediante su identificador único.
    /// 
    /// <para><strong>Información detallada incluida:</strong></para>
    /// <list type="bullet">
    /// <item><description>📋 Información completa del proyecto</description></item>
    /// <item><description>👥 Equipo de investigación completo</description></item>
    /// <item><description>📈 Cronograma y hitos del proyecto</description></item>
    /// <item><description>💰 Detalles de financiamiento</description></item>
    /// <item><description>📄 Recursos y documentación</description></item>
    /// <item><description>🎯 Objetivos y metodología</description></item>
    /// </list>
    /// 
    /// <para><strong>💡 Uso típico:</strong> Vista detallada, edición, seguimiento de progreso</para>
    /// </remarks>
    /// <param name="id">Identificador único de la investigación</param>
    /// <returns>Información completa de la investigación</returns>
    /// <response code="200">✅ Investigación encontrada exitosamente</response>
    /// <response code="404">❌ Investigación no encontrada</response>
    /// <response code="500">💥 Error interno del servidor</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(InvestigacionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Obtener investigación por ID",
        Description = "Recupera la información detallada de una investigación específica",
        OperationId = "GetInvestigacionById",
        Tags = new[] { "🔬 Investigaciones" }
    )]
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
    /// 🔍 Buscar investigaciones por cédula del investigador
    /// </summary>
    /// <remarks>
    /// Recupera todas las investigaciones en las que participa un investigador específico identificado por su cédula.
    /// 
    /// <para><strong>Tipos de participación incluidos:</strong></para>
    /// <list type="bullet">
    /// <item><description>👨‍🔬 Investigador principal</description></item>
    /// <item><description>👥 Co-investigador</description></item>
    /// <item><description>🎓 Investigador colaborador</description></item>
    /// <item><description>📚 Asistente de investigación</description></item>
    /// </list>
    /// 
    /// <para><strong>💡 Casos de uso:</strong></para>
    /// <list type="bullet">
    /// <item><description>📊 Portfolio personal de investigación</description></item>
    /// <item><description>📈 Evaluación de productividad</description></item>
    /// <item><description>🎯 Asignación de recursos</description></item>
    /// <item><description>📋 Reportes académicos individuales</description></item>
    /// </list>
    /// 
    /// <para><strong>📄 Ejemplo de búsqueda:</strong> <c>GET /api/Investigaciones/by-cedula/1234567890</c></para>
    /// </remarks>
    /// <param name="cedula">Número de cédula del investigador</param>
    /// <returns>Lista de investigaciones del investigador especificado</returns>
    /// <response code="200">✅ Investigaciones encontradas exitosamente</response>
    /// <response code="400">❌ Cédula con formato inválido</response>
    /// <response code="404">❌ No se encontraron investigaciones para esta cédula</response>
    /// <response code="500">💥 Error interno del servidor</response>
    [HttpGet("by-cedula/{cedula}")]
    [ProducesResponseType(typeof(IEnumerable<InvestigacionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Buscar investigaciones por cédula",
        Description = "Recupera todas las investigaciones asociadas a un investigador específico",
        OperationId = "GetInvestigacionesByCedula",
        Tags = new[] { "🔍 Búsquedas" }
    )]
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
    /// 🏷️ Filtrar investigaciones por tipo de proyecto
    /// </summary>
    /// <remarks>
    /// Recupera investigaciones filtradas por su tipo o categoría específica.
    /// 
    /// <para><strong>Tipos de investigación soportados:</strong></para>
    /// <list type="bullet">
    /// <item><description>🔬 Investigación Básica</description></item>
    /// <item><description>🛠️ Investigación Aplicada</description></item>
    /// <item><description>🚀 Desarrollo e Innovación</description></item>
    /// <item><description>📚 Investigación Documental</description></item>
    /// <item><description>🌐 Investigación Interdisciplinaria</description></item>
    /// </list>
    /// 
    /// <para><strong>💡 Casos de uso:</strong></para>
    /// <list type="bullet">
    /// <item><description>📊 Análisis por categorías</description></item>
    /// <item><description>📈 Reportes especializados</description></item>
    /// <item><description>🎯 Asignación de recursos por tipo</description></item>
    /// </list>
    /// </remarks>
    /// <param name="tipo">Tipo de investigación a filtrar</param>
    /// <returns>Lista de investigaciones del tipo especificado</returns>
    /// <response code="200">✅ Investigaciones filtradas exitosamente</response>
    /// <response code="400">❌ Tipo de investigación no válido</response>
    /// <response code="500">💥 Error interno del servidor</response>
    [HttpGet("by-tipo/{tipo}")]
    [ProducesResponseType(typeof(IEnumerable<InvestigacionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Filtrar investigaciones por tipo",
        Description = "Recupera investigaciones de un tipo o categoría específica",
        OperationId = "GetInvestigacionesByTipo",
        Tags = new[] { "🔍 Búsquedas" }
    )]
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
    /// 🎓 Filtrar investigaciones por campo de conocimiento
    /// </summary>
    /// <remarks>
    /// Recupera investigaciones organizadas por área disciplinar o campo de conocimiento específico.
    /// 
    /// <para><strong>Campos de conocimiento principales:</strong></para>
    /// <list type="bullet">
    /// <item><description>💻 Tecnologías de la Información</description></item>
    /// <item><description>🏥 Ciencias de la Salud</description></item>
    /// <item><description>🔬 Ciencias Exactas y Naturales</description></item>
    /// <item><description>👥 Ciencias Sociales</description></item>
    /// <item><description>🏭 Ingeniería y Tecnología</description></item>
    /// <item><description>📚 Humanidades</description></item>
    /// <item><description>🌾 Ciencias Agrícolas</description></item>
    /// </list>
    /// 
    /// <para><strong>💡 Aplicaciones:</strong></para>
    /// <list type="bullet">
    /// <item><description>📊 Reportes por facultades</description></item>
    /// <item><description>🎯 Análisis disciplinar</description></item>
    /// <item><description>📈 Distribución de recursos académicos</description></item>
    /// <item><description>🤝 Identificación de colaboraciones interdisciplinarias</description></item>
    /// </list>
    /// </remarks>
    /// <param name="campoConocimiento">Campo o área de conocimiento a filtrar</param>
    /// <returns>Lista de investigaciones del campo especificado</returns>
    /// <response code="200">✅ Investigaciones del campo obtenidas exitosamente</response>
    /// <response code="400">❌ Campo de conocimiento no reconocido</response>
    /// <response code="500">💥 Error interno del servidor</response>
    [HttpGet("by-campo/{campoConocimiento}")]
    [ProducesResponseType(typeof(IEnumerable<InvestigacionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Filtrar investigaciones por campo de conocimiento",
        Description = "Recupera investigaciones de un área disciplinar específica",
        OperationId = "GetInvestigacionesByCampo",
        Tags = new[] { "🔍 Búsquedas" }
    )]
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
    /// ➕ Crear nueva investigación
    /// </summary>
    /// <remarks>
    /// Registra un nuevo proyecto de investigación en el sistema con información básica.
    /// 
    /// <para><strong>Datos requeridos para la creación:</strong></para>
    /// <list type="bullet">
    /// <item><description>📝 Título descriptivo del proyecto</description></item>
    /// <item><description>📄 Resumen ejecutivo</description></item>
    /// <item><description>👤 Investigador principal (cédula)</description></item>
    /// <item><description>🏷️ Tipo de investigación</description></item>
    /// <item><description>🎓 Campo de conocimiento</description></item>
    /// <item><description>📅 Fechas de inicio y finalización estimadas</description></item>
    /// <item><description>💰 Presupuesto estimado</description></item>
    /// </list>
    /// 
    /// <para><strong>Validaciones aplicadas:</strong></para>
    /// <list type="bullet">
    /// <item><description>✅ Investigador principal debe existir</description></item>
    /// <item><description>📅 Fechas coherentes</description></item>
    /// <item><description>💰 Presupuesto positivo</description></item>
    /// <item><description>📝 Título único</description></item>
    /// </list>
    /// 
    /// <para><strong>💡 Ejemplo de payload:</strong></para>
    /// <code>
    /// {
    ///   "titulo": "Desarrollo de Algoritmos IA para Educación",
    ///   "resumen": "Investigación sobre aplicación de IA...",
    ///   "investigadorPrincipalCedula": "1234567890",
    ///   "tipo": "Investigación Aplicada",
    ///   "campoConocimiento": "Tecnologías de la Información",
    ///   "fechaInicio": "2024-01-15",
    ///   "fechaFin": "2025-12-31",
    ///   "presupuesto": 50000.00
    /// }
    /// </code>
    /// </remarks>
    /// <param name="createDto">Datos para crear la nueva investigación</param>
    /// <returns>Investigación creada con su ID asignado</returns>
    /// <response code="201">✅ Investigación creada exitosamente</response>
    /// <response code="400">❌ Datos inválidos o incompletos</response>
    /// <response code="409">⚠️ Conflicto - Título ya existe</response>
    /// <response code="500">💥 Error interno del servidor</response>
    [HttpPost]
    [ProducesResponseType(typeof(InvestigacionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Crear nueva investigación",
        Description = "Registra un nuevo proyecto de investigación con información básica",
        OperationId = "CreateInvestigacion",
        Tags = new[] { "🔬 Investigaciones" }
    )]
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
    /// 📄 Crear investigación con documento PDF adjunto
    /// </summary>
    /// <remarks>
    /// Registra un nuevo proyecto de investigación incluyendo un archivo PDF como documento de soporte.
    /// 
    /// <para><strong>Características del archivo PDF:</strong></para>
    /// <list type="bullet">
    /// <item><description>📎 Formato: Solo archivos PDF</description></item>
    /// <item><description>📏 Tamaño máximo: 10 MB</description></item>
    /// <item><description>📝 Contenido: Propuesta, metodología, o plan de investigación</description></item>
    /// <item><description>🔒 Almacenamiento seguro en el servidor</description></item>
    /// </list>
    /// 
    /// <para><strong>Datos de formulario requeridos:</strong></para>
    /// <list type="bullet">
    /// <item><description>📝 Información básica de la investigación</description></item>
    /// <item><description>📄 Archivo PDF (multipart/form-data)</description></item>
    /// <item><description>👤 Datos del investigador principal</description></item>
    /// </list>
    /// 
    /// <para><strong>⚠️ Consideraciones importantes:</strong></para>
    /// <list type="bullet">
    /// <item><description>📎 Usar Content-Type: multipart/form-data</description></item>
    /// <item><description>🔒 El archivo se almacena de forma segura</description></item>
    /// <item><description>📋 Se genera un ID único para el documento</description></item>
    /// </list>
    /// 
    /// <para><strong>💡 Caso de uso típico:</strong> Registro de propuestas de investigación con documentación completa</para>
    /// </remarks>
    /// <param name="dto">Datos de la investigación incluyendo archivo PDF</param>
    /// <returns>Investigación creada con referencia al documento adjunto</returns>
    /// <response code="201">✅ Investigación con PDF creada exitosamente</response>
    /// <response code="400">❌ Datos inválidos o archivo no válido</response>
    /// <response code="413">📁 Archivo demasiado grande</response>
    /// <response code="415">📄 Tipo de archivo no soportado</response>
    /// <response code="500">💥 Error interno del servidor</response>
    [HttpPost("con-pdf")]
    [ProducesResponseType(typeof(InvestigacionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status413PayloadTooLarge)]
    [ProducesResponseType(typeof(object), StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Crear investigación con documento PDF",
        Description = "Registra una nueva investigación incluyendo un archivo PDF como documento de soporte",
        OperationId = "CreateInvestigacionWithPdf",
        Tags = new[] { "🔬 Investigaciones" }
    )]
    public async Task<IActionResult> CreateWithPdf([FromForm] CreateInvestigacionWithPdfDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Log para depuración
            Console.WriteLine($"CreateWithPdf - Archivo recibido: {dto.ArchivoPdf?.FileName ?? "null"}, Tamaño: {dto.ArchivoPdf?.Length ?? 0}");

            var investigacion = await _investigacionService.CreateWithPdfAsync(dto);
            
            Console.WriteLine($"CreateWithPdf - Investigación creada exitosamente con ID: {investigacion.Id}");
            
            return CreatedAtAction(nameof(GetById), new { id = investigacion.Id }, investigacion);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CreateWithPdf - Error: {ex.Message}");
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// ✏️ Actualizar investigación existente
    /// </summary>
    /// <remarks>
    /// Modifica los datos de una investigación existente con validación completa de cambios.
    /// 
    /// <para><strong>Campos actualizables:</strong></para>
    /// <list type="bullet">
    /// <item><description>📝 Título y descripción del proyecto</description></item>
    /// <item><description>👥 Equipo de investigación</description></item>
    /// <item><description>📅 Fechas y cronograma</description></item>
    /// <item><description>💰 Presupuesto y financiamiento</description></item>
    /// <item><description>🎯 Estado del proyecto</description></item>
    /// <item><description>📊 Resultados y avances</description></item>
    /// </list>
    /// 
    /// <para><strong>Validaciones de integridad:</strong></para>
    /// <list type="bullet">
    /// <item><description>✅ ID de URL debe coincidir con ID del objeto</description></item>
    /// <item><description>🔍 Investigación debe existir</description></item>
    /// <item><description>📅 Fechas coherentes</description></item>
    /// <item><description>👤 Investigador principal válido</description></item>
    /// </list>
    /// 
    /// <para><strong>⚠️ Restricciones:</strong></para>
    /// <list type="bullet">
    /// <item><description>🚫 No se puede cambiar el ID</description></item>
    /// <item><description>📅 No se puede retroceder fechas críticas</description></item>
    /// <item><description>🔒 Algunos campos requieren permisos especiales</description></item>
    /// </list>
    /// </remarks>
    /// <param name="id">ID de la investigación a actualizar</param>
    /// <param name="updateDto">Nuevos datos de la investigación</param>
    /// <returns>Investigación actualizada con los nuevos datos</returns>
    /// <response code="200">✅ Investigación actualizada exitosamente</response>
    /// <response code="400">❌ Datos inválidos o ID no coincidente</response>
    /// <response code="404">❌ Investigación no encontrada</response>
    /// <response code="409">⚠️ Conflicto en la actualización</response>
    /// <response code="500">💥 Error interno del servidor</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(InvestigacionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Actualizar investigación existente",
        Description = "Modifica los datos de una investigación con validación completa",
        OperationId = "UpdateInvestigacion",
        Tags = new[] { "🔬 Investigaciones" }
    )]
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
    /// 📄 Actualizar investigación con nuevo documento PDF
    /// </summary>
    /// <remarks>
    /// Modifica una investigación existente incluyendo la actualización o reemplazo del documento PDF asociado.
    /// 
    /// <para><strong>Funcionalidades del endpoint:</strong></para>
    /// <list type="bullet">
    /// <item><description>✏️ Actualizar datos de la investigación</description></item>
    /// <item><description>🔄 Reemplazar documento PDF existente</description></item>
    /// <item><description>📎 Agregar PDF si no existía previamente</description></item>
    /// <item><description>🗑️ Eliminar PDF anterior de forma segura</description></item>
    /// </list>
    /// 
    /// <para><strong>Características del nuevo archivo:</strong></para>
    /// <list type="bullet">
    /// <item><description>📄 Formato: Solo archivos PDF</description></item>
    /// <item><description>📏 Tamaño máximo: 10 MB</description></item>
    /// <item><description>🔒 Almacenamiento seguro</description></item>
    /// <item><description>📝 Versionado automático</description></item>
    /// </list>
    /// 
    /// <para><strong>⚠️ Proceso de actualización:</strong></para>
    /// <list type="number">
    /// <item><description>Validar datos y archivo</description></item>
    /// <item><description>Respaldar PDF anterior</description></item>
    /// <item><description>Subir nuevo archivo</description></item>
    /// <item><description>Actualizar referencias</description></item>
    /// <item><description>Confirmar cambios</description></item>
    /// </list>
    /// </remarks>
    /// <param name="id">ID de la investigación a actualizar</param>
    /// <param name="updateDto">Datos actualizados incluyendo nuevo PDF</param>
    /// <returns>Investigación actualizada con referencia al nuevo documento</returns>
    /// <response code="200">✅ Investigación y PDF actualizados exitosamente</response>
    /// <response code="400">❌ Datos inválidos o archivo no válido</response>
    /// <response code="404">❌ Investigación no encontrada</response>
    /// <response code="413">📁 Archivo demasiado grande</response>
    /// <response code="415">📄 Tipo de archivo no soportado</response>
    /// <response code="500">💥 Error interno del servidor</response>
    [HttpPut("{id}/con-pdf")]
    [ProducesResponseType(typeof(InvestigacionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status413PayloadTooLarge)]
    [ProducesResponseType(typeof(object), StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Actualizar investigación con nuevo PDF",
        Description = "Modifica una investigación existente reemplazando su documento PDF asociado",
        OperationId = "UpdateInvestigacionWithPdf",
        Tags = new[] { "🔬 Investigaciones" }
    )]
    public async Task<ActionResult<InvestigacionDto>> UpdateWithPdf(int id, [FromForm] UpdateInvestigacionWithPdfDto updateDto)
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

            // Log para depuración
            Console.WriteLine($"UpdateWithPdf - ID: {id}, Archivo recibido: {updateDto.ArchivoPdf?.FileName ?? "null"}, Tamaño: {updateDto.ArchivoPdf?.Length ?? 0}");

            var investigacion = await _investigacionService.UpdateWithPdfAsync(updateDto);
            
            Console.WriteLine($"UpdateWithPdf - Investigación actualizada exitosamente con ID: {investigacion.Id}");
            
            return Ok(investigacion);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en UpdateWithPdf - ID: {id}, Error: {ex.Message}");
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }

    /// <summary>
    /// 🗑️ Eliminar investigación (eliminación suave)
    /// </summary>
    /// <remarks>
    /// Elimina una investigación del sistema utilizando eliminación suave (soft delete) para preservar la integridad histórica.
    /// 
    /// <para><strong>Características de la eliminación suave:</strong></para>
    /// <list type="bullet">
    /// <item><description>📋 La investigación se marca como eliminada, no se borra físicamente</description></item>
    /// <item><description>🔒 Se preservan todos los datos históricos</description></item>
    /// <item><description>📊 Mantiene integridad referencial</description></item>
    /// <item><description>🔄 Posibilidad de restauración futura</description></item>
    /// </list>
    /// 
    /// <para><strong>⚠️ Consideraciones importantes:</strong></para>
    /// <list type="bullet">
    /// <item><description>🚨 Acción requiere permisos administrativos</description></item>
    /// <item><description>📄 Los documentos PDF asociados se preservan</description></item>
    /// <item><description>👥 No afecta a los investigadores asociados</description></item>
    /// <item><description>📈 Las estadísticas históricas se mantienen</description></item>
    /// </list>
    /// 
    /// <para><strong>💡 Alternativas recomendadas:</strong></para>
    /// <list type="bullet">
    /// <item><description>⏸️ Cambiar estado a "Suspendida" en lugar de eliminar</description></item>
    /// <item><description>📝 Agregar observaciones sobre el motivo</description></item>
    /// <item><description>🔄 Considerar transferencia a otro investigador</description></item>
    /// </list>
    /// </remarks>
    /// <param name="id">ID de la investigación a eliminar</param>
    /// <returns>Confirmación de eliminación</returns>
    /// <response code="204">✅ Investigación eliminada exitosamente</response>
    /// <response code="400">❌ No se pudo procesar la eliminación</response>
    /// <response code="404">❌ Investigación no encontrada</response>
    /// <response code="403">🔒 Sin permisos para eliminar</response>
    /// <response code="500">💥 Error interno del servidor</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Eliminar investigación",
        Description = "Elimina una investigación usando eliminación suave para preservar integridad histórica",
        OperationId = "DeleteInvestigacion",
        Tags = new[] { "🔬 Investigaciones" }
    )]
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

    /// <summary>
    /// 📄 Descargar documento PDF de investigación
    /// </summary>
    /// <remarks>
    /// Descarga el archivo PDF asociado a una investigación específica de forma segura.
    /// 
    /// <para><strong>Características de la descarga:</strong></para>
    /// <list type="bullet">
    /// <item><description>📄 Formato: Archivo PDF nativo</description></item>
    /// <item><description>🔒 Descarga segura y validada</description></item>
    /// <item><description>📝 Nombre de archivo descriptivo</description></item>
    /// <item><description>⚡ Streaming para archivos grandes</description></item>
    /// </list>
    /// 
    /// <para><strong>Control de acceso:</strong></para>
    /// <list type="bullet">
    /// <item><description>🔍 Verificación de existencia de investigación</description></item>
    /// <item><description>📄 Validación de disponibilidad del PDF</description></item>
    /// <item><description>👤 Control de permisos de acceso</description></item>
    /// </list>
    /// 
    /// <para><strong>💡 Casos de uso:</strong></para>
    /// <list type="bullet">
    /// <item><description>📚 Revisión de propuestas</description></item>
    /// <item><description>📊 Evaluación de investigaciones</description></item>
    /// <item><description>🎯 Auditoría y seguimiento</description></item>
    /// <item><description>📋 Generación de reportes</description></item>
    /// </list>
    /// 
    /// <para><strong>📥 Ejemplo de uso:</strong> <c>GET /api/Investigaciones/123/pdf</c></para>
    /// </remarks>
    /// <param name="id">ID de la investigación cuyo PDF se desea descargar</param>
    /// <returns>Archivo PDF para descarga directa</returns>
    /// <response code="200">✅ PDF descargado exitosamente</response>
    /// <response code="404">❌ Investigación o PDF no encontrado</response>
    /// <response code="403">🔒 Sin permisos para acceder al documento</response>
    /// <response code="500">💥 Error interno del servidor</response>
    [HttpGet("{id}/pdf")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Descargar PDF de investigación",
        Description = "Descarga el archivo PDF asociado a una investigación específica",
        OperationId = "GetInvestigacionPdf",
        Tags = new[] { "📁 Archivos" }
    )]
    public async Task<IActionResult> GetPdf(int id)
    {
        try
        {
            var pdfBytes = await _investigacionService.GetPdfByIdAsync(id);
            
            // Log para depuración
            Console.WriteLine($"GetPdf - ID: {id}, PDF bytes: {pdfBytes?.Length ?? 0}");
            
            if (pdfBytes == null || pdfBytes.Length == 0)
                return NotFound(new { message = "PDF no encontrado para esta investigación" });

            return File(pdfBytes, "application/pdf", $"investigacion_{id}.pdf");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en GetPdf - ID: {id}, Error: {ex.Message}");
            return StatusCode(500, new { message = "Error interno del servidor", error = ex.Message });
        }
    }
}
