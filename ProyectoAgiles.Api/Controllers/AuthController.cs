using Microsoft.AspNetCore.Mvc;
using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Application.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ProyectoAgiles.Api.Controllers;

/// <summary>
/// Controlador para la gestión de autenticación y autorización de usuarios
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Tags("Autenticación")]
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
    /// Registra un nuevo usuario en el sistema
    /// </summary>
    /// <param name="registerDto">Datos del usuario a registrar</param>
    /// <returns>Información del usuario registrado</returns>
    /// <response code="200">Usuario registrado exitosamente</response>
    /// <response code="400">Datos de entrada inválidos</response>
    /// <response code="409">El usuario ya existe</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
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
