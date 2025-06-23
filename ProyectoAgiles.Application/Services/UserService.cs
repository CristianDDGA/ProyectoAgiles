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
            return null;        // Verificar si el email ya existe (excluyendo el usuario actual)
        var existingUser = await _userRepository.GetByEmailAsync(updateDto.Email);
        if (existingUser != null && existingUser.Id != id)
        {
            throw new InvalidOperationException("El email ya está registrado");
        }

        // Verificar si la cédula ya existe (excluyendo el usuario actual)
        var existingUserByCedula = await _userRepository.FindFirstAsync(u => u.Cedula == updateDto.Cedula);
        if (existingUserByCedula != null && existingUserByCedula.Id != id)
        {
            throw new InvalidOperationException("La cédula ya está registrada");
        }

        // Actualizar campos
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
    }    public async Task<bool> ToggleUserStatusAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return false;

        user.IsActive = !user.IsActive;
        await _userRepository.UpdateAsync(user);
        return true;
    }    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        // Obtener todos los usuarios
        var allUsers = await _userRepository.GetAllAsync();
        
        // Calcular estadísticas
        var totalTeachers = allUsers.Count(u => u.UserType == Domain.Enums.UserType.Docente);
        var activeUsers = allUsers.Count(u => u.IsActive);
        var todayRegistrations = allUsers.Count(u => u.CreatedAt.Date == DateTime.Today);

        return new DashboardStatsDto
        {
            TotalTeachers = totalTeachers,
            ActiveUsers = activeUsers,
            TodayRegistrations = todayRegistrations
        };
    }

    public async Task<IEnumerable<ActivityItemDto>> GetRecentActivitiesAsync()
    {
        // Obtener usuarios registrados recientemente (últimas 24 horas)
        var recentUsers = await _userRepository.GetAllAsync();
        var activities = new List<ActivityItemDto>();

        // Filtrar usuarios de las últimas 24 horas
        var recentRegistrations = recentUsers
            .Where(u => u.CreatedAt >= DateTime.Now.AddDays(-1))
            .OrderByDescending(u => u.CreatedAt)
            .Take(10);

        foreach (var user in recentRegistrations)
        {
            var userTypeText = user.UserType == Domain.Enums.UserType.Admin ? "administrador" : "docente";
            activities.Add(new ActivityItemDto
            {
                Type = "user_registered",
                Description = $"Nuevo {userTypeText} registrado: {user.FullName}",
                Timestamp = user.CreatedAt,
                Icon = "fa-user-plus"
            });
        }

        // Agregar algunas actividades del sistema simuladas si no hay muchas actividades recientes
        if (activities.Count < 3)
        {
            activities.Add(new ActivityItemDto
            {
                Type = "system",
                Description = "Sistema iniciado correctamente",
                Timestamp = DateTime.Now.AddHours(-2),
                Icon = "fa-cog"
            });

            activities.Add(new ActivityItemDto
            {
                Type = "system",
                Description = "Base de datos optimizada",
                Timestamp = DateTime.Now.AddHours(-6),
                Icon = "fa-database"
            });
        }

        return activities.OrderByDescending(a => a.Timestamp).Take(10);
    }

    public async Task<bool> UpdateUserNivelAsync(int id, string nuevoNivel)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return false;
        user.Nivel = nuevoNivel;
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
            FullName = user.FullName,
            Nivel = user.Nivel // Mapeo de nivel
        };
    }
}
