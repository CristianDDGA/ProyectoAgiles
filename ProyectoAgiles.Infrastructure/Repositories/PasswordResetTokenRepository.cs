using Microsoft.EntityFrameworkCore;
using ProyectoAgiles.Domain.Entities;
using ProyectoAgiles.Domain.Interfaces;
using ProyectoAgiles.Infrastructure.Data;

namespace ProyectoAgiles.Infrastructure.Repositories;

public class PasswordResetTokenRepository : Repository<PasswordResetToken>, IPasswordResetTokenRepository
{
    public PasswordResetTokenRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<PasswordResetToken?> GetValidTokenAsync(string token, string email)
    {
        return await _context.PasswordResetTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => 
                t.Token == token && 
                t.User.Email == email &&
                !t.IsUsed &&
                t.ExpiryDate > DateTime.UtcNow);
    }

    public async Task CleanupExpiredTokensAsync()
    {
        var expiredTokens = await _context.PasswordResetTokens
            .Where(t => t.ExpiryDate <= DateTime.UtcNow || t.IsUsed)
            .ToListAsync();

        _context.PasswordResetTokens.RemoveRange(expiredTokens);
        await _context.SaveChangesAsync();
    }
}
