using Microsoft.AspNetCore.Mvc;
using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Application.Interfaces;

namespace ProyectoAgiles.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
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
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] RegisterDto updateDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.UpdateUserAsync(id, updateDto);
            if (user == null)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            return Ok(user);
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

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        try
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpPatch("{id}/toggle-status")]
    public async Task<ActionResult> ToggleUserStatus(int id)
    {
        try
        {
            var result = await _userService.ToggleUserStatusAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }

            return Ok(new { message = "Estado del usuario actualizado correctamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpPost("{id}/subir-nivel")]
    public async Task<IActionResult> SubirNivel(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();
        if (user.UserType != ProyectoAgiles.Domain.Enums.UserType.Docente)
            return BadRequest("Solo los docentes pueden subir de nivel.");

        // Lógica simple: cambiar el nivel a un valor superior (ejemplo)
        var niveles = new[] { "titular auxiliar 1", "titular auxiliar 2", "titular principal", "titular agregado" };
        var actual = Array.IndexOf(niveles, user.Nivel);
        if (actual < 0 || actual == niveles.Length - 1)
            return BadRequest("Ya tienes el nivel más alto o nivel desconocido.");
        user.Nivel = niveles[actual + 1];
        await _userService.UpdateUserNivelAsync(id, user.Nivel);
        return Ok();
    }

    [HttpGet("by-cedula/{cedula}")]
    public async Task<ActionResult<UserDto>> GetUserByCedula(string cedula)
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            var user = users.FirstOrDefault(u => u.Cedula == cedula);
            
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
    }

    [HttpPost("by-cedula/{cedula}/subir-nivel")]
    public async Task<IActionResult> SubirNivelPorCedula(string cedula)
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            var user = users.FirstOrDefault(u => u.Cedula == cedula);
            
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });
                
            if (user.UserType != ProyectoAgiles.Domain.Enums.UserType.Docente)
                return BadRequest(new { message = "Solo los docentes pueden subir de nivel." });

            // Lógica para subir de nivel
            var niveles = new[] { "titular auxiliar 1", "titular auxiliar 2", "titular principal", "titular agregado" };
            var nivelActual = user.Nivel?.ToLower() ?? "titular auxiliar 1";
            var actual = Array.IndexOf(niveles, nivelActual);
            
            if (actual < 0)
            {
                // Si no se encuentra el nivel, asumir titular auxiliar 1
                actual = 0;
            }
            
            if (actual == niveles.Length - 1)
                return BadRequest(new { message = "Ya tienes el nivel más alto." });
                
            var nuevoNivel = niveles[actual + 1];
            await _userService.UpdateUserNivelAsync(user.Id, nuevoNivel);
            
            return Ok(new { 
                message = $"¡Felicidades! Has subido de nivel a {nuevoNivel}",
                nivelAnterior = nivelActual,
                nuevoNivel = nuevoNivel
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
        }
    }
}
