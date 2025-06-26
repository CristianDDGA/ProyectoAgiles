using Microsoft.AspNetCore.Mvc;
using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Application.Interfaces;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace ProyectoAgiles.Api.Controllers;

/// <summary>
/// 🔐 Controlador de Autenticación y Autorización
/// </summary>
/// <remarks>
/// Este controlador maneja todas las operaciones relacionadas con la autenticación de usuarios,
/// incluyendo registro, inicio de sesión, recuperación de contraseña y gestión de tokens JWT.
/// 
/// <para>
/// <strong>Funcionalidades principales:</strong>
/// </para>
/// <list type="bullet">
/// <item><description>🔑 Registro de nuevos usuarios en el sistema</description></item>
/// <item><description>🚪 Inicio de sesión con credenciales</description></item>
/// <item><description>🔄 Recuperación y restablecimiento de contraseña</description></item>
/// <item><description>👤 Gestión de perfiles de usuario</description></item>
/// <item><description>🎫 Generación y validación de tokens JWT</description></item>
/// </list>
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("🔐 Autenticación")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IFileService _fileService;

    public AuthController(IAuthService authService, IFileService fileService)
    {
        _authService = authService;
        _fileService = fileService;
    }

    /// <summary>
    /// 📝 Registrar nuevo usuario
    /// </summary>
    /// <remarks>
    /// Registra un nuevo usuario en el sistema con validación completa de datos.
    /// 
    /// <para><strong>Proceso de registro:</strong></para>
    /// <list type="number">
    /// <item><description>Validación de datos de entrada</description></item>
    /// <item><description>Verificación de usuario único</description></item>
    /// <item><description>Encriptación de contraseña</description></item>
    /// <item><description>Creación del usuario en base de datos</description></item>
    /// <item><description>Generación de perfil inicial</description></item>
    /// </list>
    /// 
    /// <para><strong>Campos requeridos:</strong></para>
    /// <list type="bullet">
    /// <item><description><c>Email</c>: Correo electrónico único y válido</description></item>
    /// <item><description><c>Password</c>: Contraseña segura (mín. 8 caracteres)</description></item>
    /// <item><description><c>FirstName</c>: Nombre del usuario</description></item>
    /// <item><description><c>LastName</c>: Apellido del usuario</description></item>
    /// <item><description><c>Role</c>: Rol en el sistema (Docente, Admin, etc.)</description></item>
    /// </list>
    /// 
    /// <para><strong>💡 Ejemplo de uso:</strong></para>
    /// <code>
    /// POST /api/Auth/register
    /// {
    ///   "email": "profesor@uta.edu.ec",
    ///   "password": "MiContraseña123!",
    ///   "firstName": "Juan",
    ///   "lastName": "Pérez",
    ///   "role": "Docente",
    ///   "phoneNumber": "+593123456789"
    /// }
    /// </code>
    /// </remarks>
    /// <param name="registerDto">Datos completos del usuario a registrar</param>
    /// <returns>Información del usuario creado incluyendo ID y datos básicos</returns>
    /// <response code="201">✅ Usuario registrado exitosamente</response>
    /// <response code="400">❌ Datos de entrada inválidos o incompletos</response>
    /// <response code="409">⚠️ El email ya está registrado en el sistema</response>
    /// <response code="500">💥 Error interno del servidor</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Registrar nuevo usuario en el sistema",
        Description = "Crea una nueva cuenta de usuario con validación completa y encriptación de contraseña",
        OperationId = "RegisterUser",
        Tags = new[] { "🔐 Autenticación" }
    )]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _authService.RegisterAsync(registerDto);
            return CreatedAtAction(nameof(GetUser), new { id = user!.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// 🚪 Iniciar sesión en el sistema
    /// </summary>
    /// <remarks>
    /// Autentica a un usuario registrado mediante email y contraseña, generando un token JWT para el acceso seguro.
    /// 
    /// <para><strong>Proceso de autenticación:</strong></para>
    /// <list type="number">
    /// <item><description>Validación de formato de credenciales</description></item>
    /// <item><description>Verificación de existencia del usuario</description></item>
    /// <item><description>Validación de contraseña encriptada</description></item>
    /// <item><description>Generación de token JWT con permisos</description></item>
    /// <item><description>Retorno de datos de sesión</description></item>
    /// </list>
    /// 
    /// <para><strong>Datos de respuesta incluyen:</strong></para>
    /// <list type="bullet">
    /// <item><description><c>Token</c>: JWT para autenticación en requests posteriores</description></item>
    /// <item><description><c>User</c>: Información básica del usuario autenticado</description></item>
    /// <item><description><c>ExpiresAt</c>: Fecha y hora de expiración del token</description></item>
    /// </list>
    /// 
    /// <para><strong>💡 Ejemplo de uso:</strong></para>
    /// <code>
    /// POST /api/Auth/login
    /// {
    ///   "email": "profesor@uta.edu.ec",
    ///   "password": "MiContraseña123!"
    /// }
    /// </code>
    /// </remarks>
    /// <param name="loginDto">Credenciales de inicio de sesión (Email y Password)</param>
    /// <returns>Token JWT y información del usuario autenticado</returns>
    /// <response code="200">✅ Inicio de sesión exitoso con token generado</response>
    /// <response code="400">❌ Credenciales inválidas o datos mal formateados</response>
    /// <response code="401">🚫 Usuario no autorizado o credenciales incorrectas</response>
    /// <response code="500">💥 Error interno del servidor</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Iniciar sesión con credenciales de usuario",
        Description = "Autentica un usuario y genera un token JWT para acceso seguro al sistema",
        OperationId = "LoginUser",
        Tags = new[] { "🔐 Autenticación" }
    )]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new LoginResponse 
                { 
                    Success = false, 
                    Message = "Datos de entrada inválidos" 
                });
            }

            var response = await _authService.LoginAsync(loginDto);
            return Ok(response);
        }
        catch (Exception)
        {
            return StatusCode(500, new LoginResponse 
            { 
                Success = false, 
                Message = "Error interno del servidor"
            });
        }
    }

    /// <summary>
    /// 👤 Obtener información de usuario por ID
    /// </summary>
    /// <remarks>
    /// Recupera los datos completos de un usuario específico mediante su identificador único.
    /// 
    /// <para><strong>Información retornada:</strong></para>
    /// <list type="bullet">
    /// <item><description><c>Id</c>: Identificador único del usuario</description></item>
    /// <item><description><c>Email</c>: Correo electrónico del usuario</description></item>
    /// <item><description><c>FirstName</c>: Nombre del usuario</description></item>
    /// <item><description><c>LastName</c>: Apellido del usuario</description></item>
    /// <item><description><c>Role</c>: Rol asignado en el sistema</description></item>
    /// <item><description><c>IsActive</c>: Estado de activación de la cuenta</description></item>
    /// </list>
    /// 
    /// <para><strong>💡 Casos de uso:</strong></para>
    /// <list type="bullet">
    /// <item><description>Consulta de perfil de usuario</description></item>
    /// <item><description>Validación de datos en formularios</description></item>
    /// <item><description>Verificación de permisos y roles</description></item>
    /// </list>
    /// 
    /// <para><strong>💡 Ejemplo de uso:</strong></para>
    /// <code>
    /// GET /api/Auth/user/123
    /// </code>
    /// </remarks>
    /// <param name="id">ID único del usuario a consultar</param>
    /// <returns>Información completa del usuario solicitado</returns>
    /// <response code="200">✅ Usuario encontrado y datos retornados</response>
    /// <response code="404">❌ Usuario no encontrado con el ID especificado</response>
    /// <response code="500">💥 Error interno del servidor</response>
    [HttpGet("user/{id}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Obtener datos de usuario por ID",
        Description = "Recupera la información completa de un usuario específico del sistema",
        OperationId = "GetUserById",
        Tags = new[] { "🔐 Autenticación" }
    )]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        try
        {
            var user = await _authService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
        }
    }    /// <summary>
    /// 📧 Verificar existencia de email
    /// </summary>
    /// <remarks>
    /// Verifica si un correo electrónico ya está registrado en el sistema para evitar duplicados.
    /// 
    /// <para><strong>Casos de uso:</strong></para>
    /// <list type="bullet">
    /// <item><description>Validación en tiempo real durante el registro</description></item>
    /// <item><description>Verificación previa antes de crear cuentas</description></item>
    /// <item><description>Validación de formularios de registro</description></item>
    /// </list>
    /// 
    /// <para><strong>💡 Ejemplo de uso:</strong></para>
    /// <code>
    /// GET /api/Auth/check-email/profesor@uta.edu.ec
    /// 
    /// Respuesta:
    /// {
    ///   "exists": true
    /// }
    /// </code>
    /// </remarks>
    /// <param name="email">Dirección de correo electrónico a verificar</param>
    /// <returns>Indica si el email ya existe en el sistema</returns>
    /// <response code="200">✅ Verificación completada exitosamente</response>
    /// <response code="500">💥 Error interno del servidor</response>
    [HttpGet("check-email/{email}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Verificar si un email ya está registrado",
        Description = "Comprueba la existencia de un correo electrónico en la base de datos para evitar duplicados",
        OperationId = "CheckEmailExists",
        Tags = new[] { "🔐 Autenticación" }
    )]
    public async Task<ActionResult<bool>> CheckEmailExists(string email)
    {
        try
        {
            var exists = await _authService.EmailExistsAsync(email);
            return Ok(new { exists });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// 🆔 Verificar existencia de cédula
    /// </summary>
    /// <remarks>
    /// Verifica si una cédula de identidad ya está registrada en el sistema para garantizar la unicidad.
    /// 
    /// <para><strong>Validaciones realizadas:</strong></para>
    /// <list type="bullet">
    /// <item><description>Búsqueda exacta en base de datos</description></item>
    /// <item><description>Verificación de formato de cédula</description></item>
    /// <item><description>Prevención de registros duplicados</description></item>
    /// </list>
    /// 
    /// <para><strong>💡 Casos de uso:</strong></para>
    /// <list type="bullet">
    /// <item><description>Validación durante registro de nuevos usuarios</description></item>
    /// <item><description>Verificación de identidad única</description></item>
    /// <item><description>Prevención de fraudes por suplantación</description></item>
    /// </list>
    /// 
    /// <para><strong>💡 Ejemplo de uso:</strong></para>
    /// <code>
    /// GET /api/Auth/check-cedula/1234567890
    /// 
    /// Respuesta:
    /// {
    ///   "exists": false
    /// }
    /// </code>
    /// </remarks>
    /// <param name="cedula">Número de cédula de identidad a verificar</param>
    /// <returns>Indica si la cédula ya existe en el sistema</returns>
    /// <response code="200">✅ Verificación completada exitosamente</response>
    /// <response code="500">💥 Error interno del servidor</response>
    [HttpGet("check-cedula/{cedula}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Verificar si una cédula ya está registrada",
        Description = "Comprueba la existencia de una cédula de identidad en la base de datos para garantizar unicidad",
        OperationId = "CheckCedulaExists",
        Tags = new[] { "🔐 Autenticación" }
    )]
    public async Task<ActionResult<bool>> CheckCedulaExists(string cedula)
    {
        try
        {
            var exists = await _authService.CedulaExistsAsync(cedula);
            return Ok(new { exists });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
        }
    }    /// <summary>
    /// 🔄 Solicitar recuperación de contraseña
    /// </summary>
    /// <remarks>
    /// Inicia el proceso de recuperación de contraseña enviando un enlace de restablecimiento al email del usuario.
    /// 
    /// <para><strong>Proceso de recuperación:</strong></para>
    /// <list type="number">
    /// <item><description>Validación de formato del email</description></item>
    /// <item><description>Verificación de existencia del usuario</description></item>
    /// <item><description>Generación de token de recuperación temporal</description></item>
    /// <item><description>Envío de email con enlace de restablecimiento</description></item>
    /// <item><description>Registro de solicitud en logs de seguridad</description></item>
    /// </list>
    /// 
    /// <para><strong>Características de seguridad:</strong></para>
    /// <list type="bullet">
    /// <item><description><c>Token temporal</c>: Válido por tiempo limitado</description></item>
    /// <item><description><c>Uso único</c>: El token se invalida después del uso</description></item>
    /// <item><description><c>Encriptación</c>: Token generado con hash seguro</description></item>
    /// </list>
    /// 
    /// <para><strong>💡 Ejemplo de uso:</strong></para>
    /// <code>
    /// POST /api/Auth/forgot-password
    /// {
    ///   "email": "profesor@uta.edu.ec"
    /// }
    /// </code>
    /// </remarks>
    /// <param name="forgotPasswordDto">Email del usuario que solicita recuperar contraseña</param>
    /// <returns>Confirmación del envío del email de recuperación</returns>
    /// <response code="200">✅ Email de recuperación enviado exitosamente</response>
    /// <response code="400">❌ Email inválido o usuario no encontrado</response>
    /// <response code="500">💥 Error interno del servidor</response>
    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(ForgotPasswordResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Solicitar recuperación de contraseña por email",
        Description = "Envía un enlace de restablecimiento de contraseña al email del usuario registrado",
        OperationId = "ForgotPassword",
        Tags = new[] { "🔐 Autenticación" }
    )]
    public async Task<ActionResult<ForgotPasswordResponse>> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.ForgotPasswordAsync(forgotPasswordDto.Email);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// 🔑 Restablecer contraseña con token
    /// </summary>
    /// <remarks>
    /// Completa el proceso de restablecimiento de contraseña utilizando el token recibido por email.
    /// 
    /// <para><strong>Proceso de restablecimiento:</strong></para>
    /// <list type="number">
    /// <item><description>Validación del token de recuperación</description></item>
    /// <item><description>Verificación de expiración del token</description></item>
    /// <item><description>Validación de la nueva contraseña</description></item>
    /// <item><description>Encriptación de la nueva contraseña</description></item>
    /// <item><description>Actualización en base de datos</description></item>
    /// <item><description>Invalidación del token usado</description></item>
    /// </list>
    /// 
    /// <para><strong>Requisitos de seguridad:</strong></para>
    /// <list type="bullet">
    /// <item><description><c>Token válido</c>: Debe ser el token enviado por email</description></item>
    /// <item><description><c>No expirado</c>: Token dentro del tiempo límite</description></item>
    /// <item><description><c>Contraseña segura</c>: Cumplir políticas de seguridad</description></item>
    /// </list>
    /// 
    /// <para><strong>💡 Ejemplo de uso:</strong></para>
    /// <code>
    /// POST /api/Auth/reset-password
    /// {
    ///   "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    ///   "newPassword": "NuevaContraseña123!",
    ///   "confirmPassword": "NuevaContraseña123!"
    /// }
    /// </code>
    /// </remarks>
    /// <param name="resetPasswordDto">Token de recuperación y nueva contraseña</param>
    /// <returns>Confirmación del restablecimiento exitoso</returns>
    /// <response code="200">✅ Contraseña restablecida exitosamente</response>
    /// <response code="400">❌ Token inválido, expirado o contraseñas no coinciden</response>
    /// <response code="500">💥 Error interno del servidor</response>
    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(ResetPasswordResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Restablecer contraseña con token de recuperación",
        Description = "Actualiza la contraseña del usuario utilizando el token de seguridad recibido por email",
        OperationId = "ResetPassword",
        Tags = new[] { "🔐 Autenticación" }
    )]
    public async Task<ActionResult<ResetPasswordResponse>> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.ResetPasswordAsync(resetPasswordDto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// 🏥 Verificar estado del servicio de autenticación
    /// </summary>
    /// <remarks>
    /// Endpoint de monitoreo que verifica la disponibilidad y estado del servicio de autenticación.
    /// 
    /// <para><strong>Información de estado incluye:</strong></para>
    /// <list type="bullet">
    /// <item><description><c>Status</c>: Estado general del servicio (healthy/unhealthy)</description></item>
    /// <item><description><c>Timestamp</c>: Marca de tiempo UTC del momento de la consulta</description></item>
    /// <item><description><c>Service</c>: Identificación del servicio</description></item>
    /// </list>
    /// 
    /// <para><strong>💡 Casos de uso:</strong></para>
    /// <list type="bullet">
    /// <item><description>Monitoreo automático de servicios</description></item>
    /// <item><description>Verificación de disponibilidad antes de operaciones críticas</description></item>
    /// <item><description>Diagnóstico de problemas de conectividad</description></item>
    /// <item><description>Health checks en balanceadores de carga</description></item>
    /// </list>
    /// 
    /// <para><strong>💡 Ejemplo de respuesta:</strong></para>
    /// <code>
    /// GET /api/Auth/health
    /// 
    /// Respuesta:
    /// {
    ///   "status": "healthy",
    ///   "timestamp": "2024-01-15T10:30:00.000Z",
    ///   "service": "ProyectoAgiles.Auth"
    /// }
    /// </code>
    /// </remarks>
    /// <returns>Estado actual del servicio de autenticación</returns>
    /// <response code="200">✅ Servicio operativo y funcionando correctamente</response>
    [HttpGet("health")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Verificar estado de salud del servicio",
        Description = "Endpoint de monitoreo para verificar la disponibilidad del servicio de autenticación",
        OperationId = "HealthCheck",
        Tags = new[] { "🔐 Autenticación" }
    )]
    public ActionResult Health()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }
    
    /// <summary>
    /// 📄 Descargar documento o archivo
    /// </summary>
    /// <remarks>
    /// Permite descargar documentos y archivos almacenados en el sistema de forma segura.
    /// 
    /// <para><strong>Tipos de archivos soportados:</strong></para>
    /// <list type="bullet">
    /// <item><description><c>PDF</c>: Documentos en formato PDF</description></item>
    /// <item><description><c>Imágenes</c>: JPG, JPEG, PNG, GIF, BMP</description></item>
    /// <item><description><c>Otros</c>: Archivos binarios diversos</description></item>
    /// </list>
    /// 
    /// <para><strong>Características de seguridad:</strong></para>
    /// <list type="bullet">
    /// <item><description>Validación de existencia del archivo</description></item>
    /// <item><description>Restricción a directorio de uploads</description></item>
    /// <item><description>Detección automática de content-type</description></item>
    /// <item><description>Prevención de path traversal attacks</description></item>
    /// </list>
    /// 
    /// <para><strong>💡 Casos de uso:</strong></para>
    /// <list type="bullet">
    /// <item><description>Descarga de documentos de usuario</description></item>
    /// <item><description>Visualización de imágenes de perfil</description></item>
    /// <item><description>Acceso a archivos adjuntos</description></item>
    /// </list>
    /// 
    /// <para><strong>💡 Ejemplo de uso:</strong></para>
    /// <code>
    /// GET /api/Auth/document/curriculum_vitae.pdf
    /// GET /api/Auth/document/foto_perfil.jpg
    /// </code>
    /// </remarks>
    /// <param name="fileName">Nombre del archivo a descargar (incluyendo extensión)</param>
    /// <returns>El archivo solicitado con el content-type apropiado</returns>
    /// <response code="200">✅ Archivo encontrado y descargado exitosamente</response>
    /// <response code="404">❌ Archivo no encontrado en el sistema</response>
    /// <response code="500">💥 Error interno del servidor</response>
    [HttpGet("document/{fileName}")]
    [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Descargar documento o archivo del sistema",
        Description = "Permite acceder y descargar archivos almacenados de forma segura en el servidor",
        OperationId = "GetDocument",
        Tags = new[] { "🔐 Autenticación" }
    )]
    public ActionResult GetDocument(string fileName)
    {
        try
        {
            var filePath = Path.Combine("uploads/documents", fileName);
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath);
            
            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound(new { message = "Documento no encontrado" });
            }
            
            var contentType = GetContentType(fileName);
            return PhysicalFile(fullPath, contentType);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al obtener el documento", details = ex.Message });
        }
    }
    
    private string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".pdf" => "application/pdf",
            _ => "application/octet-stream"
        };
    }
}
