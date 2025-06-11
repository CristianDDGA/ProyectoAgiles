using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Domain.Entities;

namespace ProyectoAgiles.Application.Interfaces;

public interface ITeacherManagementService
{
    Task<ExternalTeacherDto?> ValidateTeacherByCedulaAsync(string cedula);
    Task<ApiResponse<string>> RegisterTeacherAsync(TeacherRegistrationRequest request);
    Task<IEnumerable<ExternalTeacherDto>> GetAllExternalTeachersAsync();
}
