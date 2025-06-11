using Microsoft.AspNetCore.Mvc;
using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Application.Interfaces;

namespace ProyectoAgiles.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeacherManagementController : ControllerBase
{
    private readonly ITeacherManagementService _teacherManagementService;

    public TeacherManagementController(ITeacherManagementService teacherManagementService)
    {
        _teacherManagementService = teacherManagementService;
    }

    [HttpPost("validate-cedula")]
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

    [HttpPost("register")]
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

    [HttpGet("external-teachers")]
    public async Task<IActionResult> GetAllExternalTeachers()
    {
        var teachers = await _teacherManagementService.GetAllExternalTeachersAsync();
        return Ok(teachers);
    }
}
