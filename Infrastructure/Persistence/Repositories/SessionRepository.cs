using Application.Common.Interfaces.Repositories;
using Application.Common.Models.Pagination;
using Application.Features.AuthFeature.SessionManagement.Models;
using Core.Entities.Auth;
using Infrastructure.Persistence.Helpers;
using Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class SessionRepository(MyContext context)
    : Repository<Session, Guid>(context), ISessionRepository
{
    public async Task<Session> GetByTokenAsync(string token, CancellationToken ct = default)
    {
        var session = await dbSet
            .FirstOrDefaultAsync(rt => rt.RefreshToken == token, ct);

        return session;
    }

    public async Task<List<Session>> GetActiveTokensByUserIdAsync(long userId, CancellationToken ct = default)
    {
        return await dbSet
            .Where(rt => rt.UserId == userId && !rt.IsRevoked && rt.ExpireDate > DateTime.UtcNow)
            .ToListAsync(ct);
    }

    public async Task<PaginatedList<SessionDto>> Search(SessionIm search, CancellationToken ct)
    {
        var userQuery = context.Users.AsNoTracking();

        if (search.UserId is not null)
            userQuery = userQuery.Where(x => x.Id == search.UserId);

        if (!string.IsNullOrEmpty(search.UserFName))
            userQuery = userQuery.Where(x => x.FName.Contains(search.UserFName));

        if (!string.IsNullOrEmpty(search.UserLName))
            userQuery = userQuery.Where(x => x.LName.Contains(search.UserLName));

        var sessionQuery = context.Sessions
            .AsNoTracking()
            .Where(x => x.ExpireDate >= DateTime.UtcNow && !x.IsRevoked);

        if (search.FromDate is not null)
            sessionQuery = sessionQuery.Where(x => x.CreatedDate >= search.FromDate);

        if (search.ToDate is not null)
            sessionQuery = sessionQuery.Where(x => x.CreatedDate <= search.ToDate);

        var query = from u in userQuery
                    join s in sessionQuery on u.Id equals s.UserId
                    select new SessionDto
                    {
                        Id = s.Id,
                        CreateDate = s.CreatedDate,
                        DeviceName = s.DeviceName,
                        ExpireDate = s.ExpireDate,
                        IsRevoked = s.IsRevoked,
                        UserFName = u.FName,
                        UserLName = u.LName
                    };

        return await query.ApplyPageBasedPaginationAsync(search, ct);

    }
}