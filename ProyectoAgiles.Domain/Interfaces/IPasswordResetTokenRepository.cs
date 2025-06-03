using ProyectoAgiles.Domain.Entities;

namespace ProyectoAgiles.Domain.Interfaces;

public interface IPasswordResetTokenRepository : IRepository<PasswordResetToken>
{
    Task<PasswordResetToken?> GetValidTokenAsync(string token, string email);
    Task CleanupExpiredTokensAsync();
}
