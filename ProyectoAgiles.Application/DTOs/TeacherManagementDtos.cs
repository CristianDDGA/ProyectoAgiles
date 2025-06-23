using Microsoft.AspNetCore.Http;

namespace ProyectoAgiles.Application.DTOs;

public class ExternalTeacherDto
{
    public int Id { get; set; }
    public string Cedula { get; set; } = string.Empty;
    public string Universidad { get; set; } = string.Empty;
    public string NombresCompletos { get; set; } = string.Empty;
    public string Nivel { get; set; } = string.Empty; // Nuevo campo para el nivel/rango
}

public class TeacherValidationRequest
{
    public string Cedula { get; set; } = string.Empty;
}

public class TeacherRegistrationRequest
{
    public string Cedula { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public IFormFile? Document { get; set; }
}
