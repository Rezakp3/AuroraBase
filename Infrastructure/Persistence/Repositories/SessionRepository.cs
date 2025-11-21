using Application.Common.Interfaces.Repositories;
using Core.Entities.Auth;
using Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class SessionRepository(MyContext context) : Repository<Session, Guid>(context), ISessionRepository
{
    public async Task<Session> GetByTokenAsync(string token, CancellationToken ct = default)
    {
        var session =  await dbSet
            .FirstOrDefaultAsync(rt => rt.RefreshToken == token, ct); 
        
        return session;
    }

    public async Task<List<Session>> GetActiveTokensByUserIdAsync(long userId, CancellationToken ct = default)
    {
        return await dbSet
            .Where(rt => rt.UserId == userId && !rt.IsRevoked && rt.ExpireDate > DateTime.UtcNow)
            .ToListAsync(ct);
    }
}