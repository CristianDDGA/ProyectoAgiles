using ProyectoAgiles.Application.DTOs;
using ProyectoAgiles.Application.Interfaces;
using ProyectoAgiles.Domain.Interfaces;

namespace ProyectoAgiles.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToDto);
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user != null ? MapToDto(user) : null;
    }

    public async Task<UserDto?> UpdateUserAsync(int id, RegisterDto updateDto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return null;

        // Verificar si el email ya existe (excluyendo el usuario actual)
        var existingUser = await _userRepository.GetByEmailAsync(updateDto.Email);
        if (existingUser != null && existingUser.Id != id)
        {
            throw new InvalidOperationException("El email ya está registrado");
        }        // Actualizar campos
        user.FirstName = updateDto.FirstName.Trim();
        user.LastName = updateDto.LastName.Trim();
        user.Email = updateDto.Email.Trim().ToLower();
        user.UserType = updateDto.UserType;
        user.Cedula = updateDto.Cedula.Trim();

        // Solo actualizar contraseña si se proporciona
        if (!string.IsNullOrEmpty(updateDto.Password))
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateDto.Password);
        }

        var updatedUser = await _userRepository.UpdateAsync(user);
        return MapToDto(updatedUser);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return false;

        await _userRepository.DeleteAsync(id);
        return true;
    }

    public async Task<bool> ToggleUserStatusAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return false;

        user.IsActive = !user.IsActive;
        await _userRepository.UpdateAsync(user);
        return true;
    }

    private static UserDto MapToDto(Domain.Entities.User user)
    {        return new UserDto
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
