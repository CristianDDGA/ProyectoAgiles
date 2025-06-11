using Microsoft.EntityFrameworkCore;
using ProyectoAgiles.Domain.Entities;
using ProyectoAgiles.Domain.Interfaces;
using ProyectoAgiles.Infrastructure.Data;

namespace ProyectoAgiles.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email.ToLower());
    }

    public async Task<User?> GetByCedulaAsync(string cedula)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Cedula == cedula);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email == email.ToLower());
    }public async Task<User?> ValidateUserAsync(string email, string password)
    {
        var user = await GetByEmailAsync(email);
        
        if (user == null || !user.IsActive)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

        return user;
    }

    public async Task<bool> CedulaExistsAsync(string cedula)
    {
        return await _dbSet.AnyAsync(u => u.Cedula == cedula);
    }

    public override async Task<User> AddAsync(User entity)
    {
        // Normalizar el email a minúsculas
        entity.Email = entity.Email.ToLower();
        
        // Hash de la contraseña
        if (!string.IsNullOrEmpty(entity.PasswordHash))
        {
            entity.PasswordHash = BCrypt.Net.BCrypt.HashPassword(entity.PasswordHash);
        }

        return await base.AddAsync(entity);
    }
}
