using ProyectoAgiles.Application.DTOs;

namespace ProyectoAgiles.Application.Interfaces;

public interface IAuthService
{
    Task<UserDto?> RegisterAsync(RegisterDto registerDto);
    Task<UserDto?> LoginAsync(LoginDto loginDto);
    Task<UserDto?> GetUserByIdAsync(int userId);
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
}

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<UserDto?> UpdateUserAsync(int id, RegisterDto updateDto);
    Task<bool> DeleteUserAsync(int id);
    Task<bool> ToggleUserStatusAsync(int id);
}
