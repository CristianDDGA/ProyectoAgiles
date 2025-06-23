using Microsoft.AspNetCore.Http;
using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Application.Interfaces;
using ProyectoAgiles.Domain.Entities;
using ProyectoAgiles.Domain.Enums;
using ProyectoAgiles.Domain.Interfaces;

namespace ProyectoAgiles.Application.Services;

public class TeacherManagementService : ITeacherManagementService
{
    private readonly IExternalTeacherRepository _externalTeacherRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITTHHRepository _tthhRepository;

    public TeacherManagementService(
        IExternalTeacherRepository externalTeacherRepository,
        IUserRepository userRepository,
        ITTHHRepository tthhRepository)
    {
        _externalTeacherRepository = externalTeacherRepository;
        _userRepository = userRepository;
        _tthhRepository = tthhRepository;
    }

    public async Task<ExternalTeacherDto?> ValidateTeacherByCedulaAsync(string cedula)
    {
        var externalTeacher = await _externalTeacherRepository.GetByCedulaAsync(cedula);
        
        if (externalTeacher == null)
            return null;

        return new ExternalTeacherDto
        {
            Id = externalTeacher.Id,
            Cedula = externalTeacher.Cedula,
            Universidad = externalTeacher.Universidad,
            NombresCompletos = externalTeacher.NombresCompletos
            // Nivel eliminado, ya no se mapea
        };
    }

    public async Task<ApiResponse<string>> RegisterTeacherAsync(TeacherRegistrationRequest request)
    {
        try
        {
            // Validar que la cédula existe en la base de datos externa
            var externalTeacher = await _externalTeacherRepository.GetByCedulaAsync(request.Cedula);
            if (externalTeacher == null)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "La cédula no se encuentra registrada en la base de datos de docentes externos."
                };
            }

            // Verificar que el email no esté ya registrado
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Ya existe un usuario con este correo electrónico."
                };
            }

            // Verificar que la cédula no esté ya registrada como usuario
            var existingUserByCedula = await _userRepository.GetByCedulaAsync(request.Cedula);
            if (existingUserByCedula != null)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = "Ya existe un usuario registrado con esta cédula."
                };
            }

            // Extraer nombres del docente externo
            var nombresParts = externalTeacher.NombresCompletos.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var firstName = nombresParts.Length > 0 ? nombresParts[0] : "";
            var lastName = nombresParts.Length > 1 ? string.Join(" ", nombresParts.Skip(1)) : "";            // Crear el nuevo usuario
            var newUser = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = request.Email,
                PasswordHash = request.Password, // No hashear aquí, el repositorio se encarga
                UserType = UserType.Docente,
                Cedula = request.Cedula,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Nivel = "titular auxiliar 1" // Nivel por defecto al registrar un docente
            };

            await _userRepository.AddAsync(newUser);

            // Registrar en TTHH
            var tthh = new TTHH
            {
                Cedula = request.Cedula,
                FechaInicio = DateTime.UtcNow,
                Observacion = "Registro automático al crear docente",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _tthhRepository.AddAsync(tthh);

            // TODO: Manejar el documento subido si es necesario
            // if (request.Document != null)
            // {
            //     // Lógica para guardar el documento
            // }

            return new ApiResponse<string>
            {
                Success = true,
                Message = "Docente registrado exitosamente.",
                Data = "Usuario creado correctamente"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = $"Error al registrar el docente: {ex.Message}"
            };
        }
    }

    public async Task<IEnumerable<ExternalTeacherDto>> GetAllExternalTeachersAsync()
    {
        var externalTeachers = await _externalTeacherRepository.GetAllAsync();
        
        return externalTeachers.Select(et => new ExternalTeacherDto
        {
            Id = et.Id,
            Cedula = et.Cedula,
            Universidad = et.Universidad,
            NombresCompletos = et.NombresCompletos
            // Nivel eliminado, ya no se mapea
        });
    }
}
