using Application.Common.Interfaces.Repositories;
using Core.Entities.Auth.Relation;
using Infrastructure.Persistence.Repositories.Base; // یا هر جایی که Base Repo هست
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserRoleRepository(MyContext context)
    : Repository<UserRole, int>(context), IUserRoleRepository
{
    public async Task<List<int>> GetUserRoleIdsAsync(long userId, CancellationToken ct = default)
    {
        return await dbSet
            .AsNoTracking()
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync(ct);
    }

    public async Task AssignRoleToUserAsync(long userId, int roleId, CancellationToken ct = default)
    {
        if (!await dbSet.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId, ct))
        {
            await dbSet.AddAsync(new UserRole { UserId = userId, RoleId = roleId }, ct);
        }
    }

    public async Task RemoveRoleFromUserAsync(long userId, int roleId, CancellationToken ct = default)
    {
        var entity = await dbSet.FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId, ct);
        if (entity != null)
        {
            dbSet.Remove(entity);
        }
    }
}