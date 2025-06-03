using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Application.Interfaces;
using ProyectoAgiles.Domain.Entities;
using ProyectoAgiles.Domain.Interfaces;

namespace ProyectoAgiles.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }    public async Task<UserDto?> RegisterAsync(RegisterDto registerDto)
    {
        // Verificar si el email ya existe
        if (await _userRepository.EmailExistsAsync(registerDto.Email))
        {
            throw new InvalidOperationException("El email ya está registrado");
        }

        // Verificar si la cédula ya existe
        if (await _userRepository.CedulaExistsAsync(registerDto.Cedula))
        {
            throw new InvalidOperationException("La cédula ya está registrada");
        }

        // Crear nuevo usuario con rol de docente
        var user = new User
        {
            FirstName = registerDto.FirstName.Trim(),
            LastName = registerDto.LastName.Trim(),
            Email = registerDto.Email.Trim().ToLower(),
            PasswordHash = registerDto.Password, // Se hashea en el repositorio
            UserType = Domain.Enums.UserType.Docente, // Asignar automáticamente rol de docente
            Cedula = registerDto.Cedula.Trim(),
            IsActive = true
        };

        var createdUser = await _userRepository.AddAsync(user);
        return MapToDto(createdUser);
    }

    public async Task<UserDto?> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.ValidateUserAsync(loginDto.Email, loginDto.Password);
        return user != null ? MapToDto(user) : null;
    }

    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user != null ? MapToDto(user) : null;
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        return user != null ? MapToDto(user) : null;
    }    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _userRepository.EmailExistsAsync(email);
    }

    public async Task<bool> CedulaExistsAsync(string cedula)
    {
        return await _userRepository.CedulaExistsAsync(cedula);
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            UserType = user.UserType,
            Cedula = user.Cedula,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            FullName = user.FullName
        };
    }
}
