using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using Application.Common.Models.Pagination;
using Application.Features.MenuFeature.Models;
using Core.Entities.Auth;
using Core.Entities.Auth.Relation;
using Infrastructure.Persistence.Helpers;
using Infrastructure.Persistence.Helpers.Cursor;
using Infrastructure.Persistence.Repositories.Base;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class MenuRepository(MyContext context)
    : Repository<Menu, int>(context), IMenuRepository
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

    public async Task<List<MenuDto>> GetMenusByRoleIds(IEnumerable<int> userRoleIds, CancellationToken ct)
    {
        var menues = await (from rm in context.RoleMenus
                            join r in userRoleIds on rm.RoleId equals r
                            join m in context.Menus on rm.MenuId equals m.Id
                            select m).Distinct().ToListAsync(ct);

        //menues = [.. menues.DistinctBy(x => x.Id)];

        return [.. Map(menues)];
    }

    public async Task<PaginatedList<MenuDto>> SearchAsync(SearchMenuDto searchMenuDto, CancellationToken cancellationToken)
    {
        var query = (from m in context.Menus
                     from pm in context.Menus.Where(x => x.Id == m.ParentId).DefaultIfEmpty()
                     select new MenuDto
                     {
                         Id = m.Id,
                         IsActive = m.IsActive,
                         Icon = m.Icon,
                         ParentId = m.ParentId,
                         ParentTitle = m.ParentId != null ? pm.Title : null,
                         Priority = m.Priority,
                         Route = m.Route,
                         Title = m.Title,
                     }).AsNoTracking();


        if (!string.IsNullOrWhiteSpace(searchMenuDto.Title))
            query = query.Where(x => x.Title.Contains(searchMenuDto.Title));
        if (!string.IsNullOrWhiteSpace(searchMenuDto.Route))
            query = query.Where(x => x.Route.Contains(searchMenuDto.Route));
        if (searchMenuDto.ParentId is not null)
            query = query.Where(x => x.ParentId == searchMenuDto.ParentId);
        if (searchMenuDto.IsActive is not null)
            query = query.Where(x => x.IsActive == searchMenuDto.IsActive);
        if (searchMenuDto.Priority is not null)
            query = query.Where(x => x.Priority == searchMenuDto.Priority);

        return await query.ApplyPageBasedPaginationAsync(searchMenuDto, cancellationToken);
    }

    private static IEnumerable<MenuDto> Map(IEnumerable<Menu> orgList, long? parentId = null, string parentTitle = default)
        => orgList
            .Where(x => x.ParentId == parentId)
            .DistinctBy(x => x.Id)
            .OrderBy(x => x.Priority)
            .Select(x => new MenuDto
            {
                Id = x.Id,
                IsActive = x.IsActive,
                Priority = x.Priority,
                ParentId = x.ParentId,
                ParentTitle = parentTitle,
                Route = x.Route,
                Title = x.Title,
                Icon = x.Icon,
                SubMenus = [.. Map(orgList, x.Id, x.Title)],
            });

    public async Task<CursorPaginatedList<BaseDropDown<int>, int>> DropDown
        (string search, CursorPagingOption<int> pagingOption, CancellationToken ct)
    {
        var query = context.Menus.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(search))
            query = query.Where(x => x.Title.Contains(search));

        var data = await query
            .Select(x => new BaseDropDown<int>
            {
                Id = x.Id,
                Value = x.Title
            }).ApplyCursorBasedPaginationAsync(pagingOption, ct);
        return data;
    }

    public async Task DeleteMenuServicesByMenuId(int menuId, CancellationToken ct)
        => await context.MenuServices.Where(x => x.MenuId == menuId).ExecuteDeleteAsync(ct);

    public async Task AddRangeServices(int menuId, IEnumerable<int> serviceIds, CancellationToken ct)
    {
        var menuServices = serviceIds.Select(serviceId => new MenuService
        {
            MenuId = menuId,
            ServiceId = serviceId
        });
        await context.MenuServices.AddRangeAsync(menuServices);
    }

    public async Task DeleteMenuRolesByMenuId(int menuId, CancellationToken ct)
        => await context.RoleMenus.Where(x => x.MenuId == menuId).ExecuteDeleteAsync(ct);

    public async Task AddRangeRoles(int menuId, IEnumerable<int> roleIds, CancellationToken ct)
    {
        var roleMenus = roleIds.Select(x => new RoleMenu
        {
            MenuId = menuId,
            RoleId = x
        });

        await context.RoleMenus.AddRangeAsync(roleMenus, ct);
    }

    public async Task<CursorPaginatedList<BaseDropDown<int>, int>> MenuRolesDropDown
        (int menuId, string roleTitle, CursorPagingOption<int> paging, CancellationToken ct)
    {
        var roleQuery = context.Roles.AsQueryable();

        if (!string.IsNullOrEmpty(roleTitle))
            roleQuery = roleQuery.Where(x => x.Title.Contains(roleTitle));

        var query = from r in roleQuery
                    from rm in context.RoleMenus.Where(x => x.MenuId == menuId && x.RoleId == r.Id).DefaultIfEmpty()
                    orderby r.Id
                    select new BaseDropDown<int>()
                    {
                        Id = r.Id,
                        Value = r.Title,
                        IsSelected = rm != null
                    };

        var data = await query.AsNoTracking().ApplyCursorBasedPaginationAsync(paging, ct);

        return data;
    }

    public async Task<CursorPaginatedList<BaseDropDown<int>, int>> MenuServicesDropDown
        (int menuId, string roleTitle, CursorPagingOption<int> paging, CancellationToken ct)
    {
        var serviceQuery = context.Services.AsQueryable();

        if (!string.IsNullOrEmpty(roleTitle))
            serviceQuery = serviceQuery.Where(x => x.ServiceName.Contains(roleTitle));

        var query = from s in context.Services
                    from ms in context.MenuServices.Where(x => x.MenuId == menuId && x.ServiceId == s.Id).DefaultIfEmpty()
                    orderby s.Id
                    select new BaseDropDown<int>()
                    {
                        Id = s.Id,
                        Value = s.ServiceName,
                        IsSelected = ms != null
                    };

        var data = await query.AsNoTracking().ApplyCursorBasedPaginationAsync(paging, ct);

        return data;
    }
}