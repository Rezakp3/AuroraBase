using Application.Common.Interfaces.Repositories;
using Application.Common.Models.Pagination;
using Application.Features.RoleFeatures.Models;
using Core.Entities.Auth;
using Core.Entities.Auth.Relation;
using Infrastructure.Persistence.Helpers;
using Infrastructure.Persistence.Repositories.Base;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class RoleRepository(MyContext context) : Repository<Role, int>(context), IRoleRepository
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

    public async Task AssignMenuesToRole(int roleId, IEnumerable<int> menuIds, CancellationToken ct)
    {
        var validMenues = await (from m in context.Menus
                                 join mi in menuIds on m.Id equals mi
                                 from mr in context.RoleMenus.Where(x => x.RoleId == roleId).DefaultIfEmpty()
                                 select new { mr, m, mi })
                                 .Where(x => x.mr == null).Select(x => x.mi).ToListAsync(ct);

        var list = validMenues.Select(x => new RoleMenu()
        {
            RoleId = roleId,
            MenuId = x
        });

        context.RoleMenus.AddRange(list);
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