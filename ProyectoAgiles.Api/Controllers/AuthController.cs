using Microsoft.AspNetCore.Mvc;
using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Application.Interfaces;

namespace ProyectoAgiles.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
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

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _authService.LoginAsync(loginDto);
            if (user == null)
            {
                return Unauthorized(new { message = "Email o contraseña incorrectos" });
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
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
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult<ForgotPasswordResponse>> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.ForgotPasswordAsync(forgotPasswordDto.Email);
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
}
