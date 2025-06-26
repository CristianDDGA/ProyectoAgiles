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
    /// Inicia sesión de un usuario en el sistema
    /// </summary>
    /// <param name="loginDto">Credenciales de inicio de sesión</param>
    /// <returns>Token de autenticación y datos del usuario</returns>
    /// <response code="200">Inicio de sesión exitoso</response>
    /// <response code="400">Credenciales inválidas</response>
    /// <response code="401">Usuario no autorizado</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

    [HttpGet("user/{id}")]
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
    }    [HttpGet("check-email/{email}")]
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

    [HttpGet("check-cedula/{cedula}")]
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
    }    [HttpPost("forgot-password")]
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

    [HttpPost("reset-password")]
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

    [HttpGet("health")]
    public ActionResult Health()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }
    
    [HttpGet("document/{fileName}")]
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
