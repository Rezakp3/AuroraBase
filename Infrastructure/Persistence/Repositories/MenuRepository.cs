using Application.Common.Interfaces.Repositories;
using Application.Common.Models.Pagination;
using Application.Features.MenuFeature.Models;
using Core.Entities.Auth;
using Infrastructure.Persistence.Helpers;
using Infrastructure.Persistence.Repositories.Base;
using Mapster;
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
        var userMenus = await (from m in context.Menus.Where(x => !x.IsActive)
                               join rm in context.RoleMenus on m.Id equals rm.MenuId
                               join r in context.UserRoles on rm.RoleId equals r.RoleId
                               where r.UserId == userId
                               select m).ToListAsync(cancellationToken);
        return userMenus;
    }

    public async Task<PaginatedList<MenuDto>> SearchAsync(SearchMenuDto searchMenuDto, CancellationToken cancellationToken)
    {
        var query = context.Menus.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchMenuDto.Title))
            query = query.Where(x => x.Title.Contains(searchMenuDto.Title));
        if (!string.IsNullOrWhiteSpace(searchMenuDto.Route))
            query = query.Where(x => x.Route.Contains(searchMenuDto.Route));
        if (searchMenuDto.ParentId is not null)
            query = query.Where(x => x.ParentId == searchMenuDto.ParentId);
        if (searchMenuDto.IsActive is not null)
            query = query.Where(x => x.IsActive == searchMenuDto.IsActive);

        return await query.ProjectToType<MenuDto>().ApplyPageBasedPaginationAsync(searchMenuDto, cancellationToken);
    }
}