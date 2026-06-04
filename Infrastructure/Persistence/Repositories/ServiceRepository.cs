using Application.Common.Interfaces.Repositories;
using Application.Common.Models.Pagination;
using Application.Features.ServiceFeatures.Models;
using Core.Entities.Auth;
using Infrastructure.Persistence.Helpers;
using Infrastructure.Persistence.Repositories.Base;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ServiceRepository(MyContext context) : Repository<Service, int>(context), IServiceRepository
{
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
}