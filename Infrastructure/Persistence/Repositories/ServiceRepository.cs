using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using Application.Common.Models.Pagination;
using Application.Features.ServiceFeatures.Models;
using Core.Entities.Auth;
using DNTPersianUtils.Core;
using Infrastructure.Persistence.Helpers;
using Infrastructure.Persistence.Repositories.Base;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ServiceRepository(MyContext context)
    : Repository<Service, int>(context), IServiceRepository
{
    public async Task<CursorPaginatedList<BaseDropDown<int>, int>> DropDown(string search, CursorPagingOption<int> paging, CancellationToken ct)
    {
        var query = dbSet.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(search.Trim()))
            query = query.Where(x => x.ServiceName.Contains(search) || x.ServiceIdentifier.Contains(search));

        return await query.Select(x => new BaseDropDown<int>()
        {
            Id = x.Id,
            Value = x.ServiceName
        }).ApplyCursorBasedPaginationAsync(paging, ct);
    }

    public async Task<PaginatedList<ServiceDto>> GetByMenuIdAsync(int menuId, PagingOption pagingOption, CancellationToken cancellationToken)
    {
        var query = from ms in context.MenuServices.Where(x => x.MenuId == menuId)
                    join s in dbSet on ms.ServiceId equals s.Id
                    select s;

        return await query.ProjectToType<ServiceDto>()
            .ApplyPageBasedPaginationAsync(pagingOption, cancellationToken);
    }

    public async Task<PaginatedList<ServiceDto>> GetByRoleIdAsync(int roleId, PagingOption pagingOption, CancellationToken cancellationToken)
    {
        var query = from rs in context.RoleServices.Where(x => x.RoleId == roleId)
                    join s in dbSet on rs.ServiceId equals s.Id
                    select s;

        return await query.ProjectToType<ServiceDto>()
            .ApplyPageBasedPaginationAsync(pagingOption, cancellationToken);
    }

    public async Task<List<ServiceDto>> GetServicesByRoleIds(IEnumerable<int> roleIds, CancellationToken ct)
        => await (from rs in context.RoleServices
                  join r in roleIds on rs.RoleId equals r
                  join s in context.Services on rs.ServiceId equals s.Id
                  select s).Distinct().ProjectToType<ServiceDto>().ToListAsync(ct);


    public async Task<PaginatedList<ServiceDto>> Search(SearchServiceIm search, CancellationToken ct)
    {
        var query = context.Services.AsNoTracking().AsQueryable();

        if (search.Address is not null)
            query = query.Where(x => x.Address.Contains(search.Address));
        if (search.Description is not null)
            query = query.Where(x => x.Description.Contains(search.Description));
        if (search.ServiceIdentifier is not null)
            query = query.Where(x => x.ServiceIdentifier.Contains(search.ServiceIdentifier));
        if (search.ServiceName is not null)
            query = query.Where(x => x.ServiceName.Contains(search.ServiceName));

        return await query.ProjectToType<ServiceDto>().ApplyPageBasedPaginationAsync(search, ct);
    }
}