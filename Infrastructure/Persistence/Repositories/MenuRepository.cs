using Application.Common.Interfaces.Repositories;
using Core.Entities.Auth;
using Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class MenuRepository(MyContext context) : Repository<Menu, int>(context), IMenuRepository
{
    public async Task<IEnumerable<Menu>> GetByRoleId(int roleId, CancellationToken ct = default)
    {
        var menues = await context.RoleMenus
            .Include(x => x.Menu)
            .AsNoTracking()
            .Where(x => x.RoleId == roleId)
            .Select(x => x.Menu).ToListAsync(ct);

        return menues;
    }

    public async Task<IEnumerable<Menu>> GetByUserId(long userId, CancellationToken cancellationToken)
    {
        var userMenus = await (from m in context.Menus
                               join rm in context.RoleMenus on m.Id equals rm.MenuId
                               join r in context.UserRoles on rm.RoleId equals r.RoleId
                               where r.UserId == userId
                               select m).ToListAsync(cancellationToken);
        return userMenus;
    }
}