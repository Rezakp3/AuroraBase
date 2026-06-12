using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using Application.Common.Models.Pagination;
using Application.Features.RoleFeatures.RoleClaimFeatures.Models;
using Application.Features.RoleFeatures.RoleManagement.Models;
using Application.Features.RoleFeatures.RoleRelations.Queries;
using Core.Entities.Auth;
using Core.Entities.Auth.Relation;
using Infrastructure.Persistence.Helpers;
using Infrastructure.Persistence.Repositories.Base;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class RoleRepository(MyContext context)
    : Repository<Role, int>(context), IRoleRepository
{
    public void AddClaims(int roleId, IEnumerable<RoleClaimDto> claims, CancellationToken cancellationToken)
    {
        var validClaims = (from c in claims
                           from rc in context.RoleClaims.Where(x => x.RoleId == roleId && c.Type == x.Type && c.Value == x.Value).DefaultIfEmpty()
                           where rc is null
                           select new RoleClaim
                           {
                               RoleId = roleId,
                               Type = c.Type,
                               Value = c.Value
                           }).ToList();

        context.RoleClaims.AddRange(validClaims);
    }

    public async Task AddRangeMenues(int roleId, IEnumerable<int> menuIds, CancellationToken ct)
    {
        var menues = menuIds.Select(x => new RoleMenu
        {
            RoleId = roleId,
            MenuId = x
        });

        await context.RoleMenus.AddRangeAsync(menues, ct);
    }

    public async Task DeleteMenues(int roleId, CancellationToken ct)
        => await context.RoleMenus.Where(x => x.RoleId == roleId).ExecuteDeleteAsync(ct);

    public void DeleteRoleClaims(IEnumerable<int> ids)
        => context.RoleClaims.RemoveRange(context.RoleClaims.Where(x => ids.Contains(x.Id)));

    public async Task<CursorPaginatedList<BaseDropDown<int>, int>> DropDown(string search, CursorPagingOption<int> pagingOption, CancellationToken ct)
    {
        var query = context.Roles.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(search))
            query = query.Where(x => x.Title.Contains(search) || x.Name.Contains(search));

        var data = await query
            .Select(x => new BaseDropDown<int>
            {
                Id = x.Id,
                Value = x.Name
            }).ApplyCursorBasedPaginationAsync(pagingOption, ct);
        return data;
    }

    public async Task<RoleClaim> GetClaimByIdAsync(int id, CancellationToken cancellationToken)
        => await context.RoleClaims.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IEnumerable<RoleClaimDto>> GetClaimsByRoleIdAsync(int roleId, CancellationToken cancellationToken)
        => await context.RoleClaims
            .Where(x => x.RoleId == roleId)
            .ProjectToType<RoleClaimDto>()
            .ToListAsync(cancellationToken);

    public async Task<IEnumerable<BaseDropDown<int>>> ServiceDropDown(int roleId, string search, CancellationToken cancellationToken)
    {
        var serviceQuery = context.Services.AsNoTracking();

        if (!string.IsNullOrEmpty(search))
            serviceQuery = serviceQuery.Where(x => x.ServiceName.Contains(search));

        var query = from s in serviceQuery
                    join rs in context.RoleServices.Where(x => x.RoleId == roleId)
                        on s.Id equals rs.ServiceId into joinedRoleService
                    from subRs in joinedRoleService.DefaultIfEmpty()
                    orderby s.Id
                    select new BaseDropDown<int>
                    {
                        Id = s.Id,
                        Value = s.ServiceName,
                        IsSelected = subRs != null
                    };

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<BaseDropDown<int>>> MenuDropDown(int roleId, string search, CancellationToken cancellationToken)
    {
        var menuQuery = context.Menus.AsNoTracking();

        if (!string.IsNullOrEmpty(search))
            menuQuery = menuQuery.Where(x => x.Title.Contains(search));

        var query = from m in menuQuery
                    join rm in context.RoleServices.Where(x => x.RoleId == roleId)
                        on m.Id equals rm.ServiceId into joinedRoleMenu
                    from subRs in joinedRoleMenu.DefaultIfEmpty()
                    orderby m.Id
                    select new BaseDropDown<int>
                    {
                        Id = m.Id,
                        Value = m.Title,
                        IsSelected = subRs != null
                    };

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<PaginatedList<RoleDto>> Search(RoleIm query, CancellationToken ct)
    {
        var q = dbSet.AsQueryable().ProjectToType<RoleDto>();

        if (!string.IsNullOrEmpty(query.Name))
            q = q.Where(x => x.Name.Contains(query.Name));
        if (!string.IsNullOrEmpty(query.Title))
            q = q.Where(x => x.Title.Contains(query.Title));

        return await PaginationHelper.ApplyPageBasedPaginationAsync(q, query, ct);
    }
}