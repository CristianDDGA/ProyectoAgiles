using ProyectoAgiles.Application.DTOs;

namespace ProyectoAgiles.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<UserDto?> UpdateUserAsync(int id, RegisterDto updateDto);
    Task<bool> DeleteUserAsync(int id);
    Task<bool> ToggleUserStatusAsync(int id);
    Task<DashboardStatsDto> GetDashboardStatsAsync();
    Task<IEnumerable<ActivityItemDto>> GetRecentActivitiesAsync();
    Task<bool> UpdateUserNivelAsync(int id, string nuevoNivel);
}
