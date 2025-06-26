using Microsoft.AspNetCore.Mvc;
using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace ProyectoAgiles.Api.Controllers;

/// <summary>
/// Controlador para la gestión de docentes
/// </summary>
/// <remarks>
/// Este controlador maneja todas las operaciones relacionadas con la gestión de docentes,
/// incluyendo validación, registro y consulta de docentes externos.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[SwaggerTag("Gestión de Docentes")]
public class TeacherManagementController : ControllerBase
{
    private readonly ITeacherManagementService _teacherManagementService;

    public TeacherManagementController(ITeacherManagementService teacherManagementService)
    {
        _teacherManagementService = teacherManagementService;
    }

    /// <summary>
    /// Validar docente por cédula
    /// </summary>
    /// <remarks>
    /// Valida si existe un docente con la cédula proporcionada en el sistema TTHH.
    /// 
    /// **Proceso de validación:**
    /// - Verifica formato y longitud de cédula
    /// - Busca en base de datos TTHH
    /// - Retorna información básica del docente si existe
    /// 
    /// **Casos de uso:**
    /// - Pre-validación antes del registro
    /// - Verificación de existencia de docente
    /// - Consulta de datos básicos para formularios
    /// </remarks>
    /// <param name="request">Objeto que contiene la cédula a validar</param>
    /// <returns>Información del docente encontrado o mensaje de error</returns>
    /// <response code="200">Docente encontrado exitosamente</response>
    /// <response code="400">Cédula no proporcionada o formato inválido</response>
    /// <response code="404">No se encontró docente con la cédula proporcionada</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost("validate-cedula")]
    [SwaggerOperation(
        Summary = "Validar docente por cédula",
        Description = "Valida la existencia de un docente en el sistema TTHH usando su número de cédula",
        Tags = new[] { "Gestión de Docentes" }
    )]
    [SwaggerResponse(200, "Docente encontrado", typeof(object))]
    [SwaggerResponse(400, "Datos de entrada inválidos")]
    [SwaggerResponse(404, "Docente no encontrado")]
    [SwaggerResponse(500, "Error interno del servidor")]
    public async Task<IActionResult> ValidateTeacherByCedula([FromBody] TeacherValidationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Cedula))
        {
            return BadRequest(new { message = "La cédula es requerida." });
        }

        var teacher = await _teacherManagementService.ValidateTeacherByCedulaAsync(request.Cedula);
        
        if (teacher == null)
        {
            return NotFound(new { message = "No se encontró un docente con la cédula proporcionada." });
        }

        return Ok(teacher);
    }

    /// <summary>
    /// Registrar nuevo docente
    /// </summary>
    /// <remarks>
    /// Registra un nuevo docente en el sistema con validación previa en TTHH.
    /// 
    /// **Proceso de registro:**
    /// 1. Validación de datos obligatorios
    /// 2. Verificación de existencia en TTHH
    /// 3. Validación de unicidad de email
    /// 4. Creación de cuenta de usuario
    /// 5. Encriptación de contraseña
    /// 6. Asignación de rol docente
    /// 
    /// **Datos requeridos:**
    /// - Cédula (debe existir en TTHH)
    /// - Email (único en el sistema)
    /// - Contraseña (mínimo 6 caracteres)
    /// - Documento CV (opcional)
    /// 
    /// **Validaciones:**
    /// - Cédula debe existir en sistema TTHH
    /// - Email no debe estar registrado
    /// - Contraseña debe cumplir políticas
    /// </remarks>
    /// <param name="request">Datos del docente a registrar incluyendo archivos</param>
    /// <returns>Resultado del proceso de registro</returns>
    /// <response code="200">Docente registrado exitosamente</response>
    /// <response code="400">Datos inválidos o docente ya registrado</response>
    /// <response code="409">Email ya existe en el sistema</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost("register")]
    [SwaggerOperation(
        Summary = "Registrar nuevo docente",
        Description = "Registra un nuevo docente en el sistema con validación previa en TTHH",
        Tags = new[] { "Gestión de Docentes" }
    )]
    [SwaggerResponse(200, "Docente registrado exitosamente")]
    [SwaggerResponse(400, "Datos de entrada inválidos")]
    [SwaggerResponse(409, "Email ya registrado")]
    [SwaggerResponse(500, "Error interno del servidor")]
    public async Task<IActionResult> RegisterTeacher([FromForm] TeacherRegistrationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Cedula) || 
            string.IsNullOrWhiteSpace(request.Email) || 
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Todos los campos son requeridos." });
        }

        var result = await _teacherManagementService.RegisterTeacherAsync(request);
        
        if (!result.Success)
        {
            return BadRequest(new { message = result.Message });
        }

        return Ok(new { message = result.Message });
    }

    /// <summary>
    /// Obtener todos los docentes externos
    /// </summary>
    /// <remarks>
    /// Obtiene la lista completa de docentes externos registrados en el sistema TTHH.
    /// 
    /// **Información incluida:**
    /// - Datos personales básicos
    /// - Información de contacto
    /// - Estado de registro en el sistema
    /// - Fecha de última actualización
    /// 
    /// **Casos de uso:**
    /// - Listado administrativo de docentes
    /// - Consulta para procesos de selección
    /// - Reportes y estadísticas
    /// - Validación de registros masivos
    /// 
    /// **Ordenamiento:**
    /// Los resultados se ordenan alfabéticamente por apellido y nombre.
    /// </remarks>
    /// <returns>Lista de todos los docentes externos</returns>
    /// <response code="200">Lista obtenida exitosamente</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("external-teachers")]
    [SwaggerOperation(
        Summary = "Obtener todos los docentes externos",
        Description = "Obtiene la lista completa de docentes externos del sistema TTHH",
        Tags = new[] { "Gestión de Docentes" }
    )]
    [SwaggerResponse(200, "Lista de docentes obtenida exitosamente", typeof(List<object>))]
    [SwaggerResponse(500, "Error interno del servidor")]
    public async Task<IActionResult> GetAllExternalTeachers()
    {
        var teachers = await _teacherManagementService.GetAllExternalTeachersAsync();
        return Ok(teachers);
    }
}
