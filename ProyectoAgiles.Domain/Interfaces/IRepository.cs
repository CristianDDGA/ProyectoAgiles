using ProyectoAgiles.Domain.Entities;
using System.Linq.Expressions;

namespace ProyectoAgiles.Domain.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FindFirstAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<int> CountAsync();
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);
}

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByCedulaAsync(string cedula);
    Task<bool> EmailExistsAsync(string email);
    Task<User?> ValidateUserAsync(string email, string password);
    Task<bool> CedulaExistsAsync(string cedula);
}