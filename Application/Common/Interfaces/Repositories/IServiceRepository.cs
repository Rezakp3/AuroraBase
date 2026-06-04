using Application.Common.Interfaces.Generals;
using Application.Common.Models.Pagination;
using Application.Features.ServiceFeatures.Models;
using Core.Entities.Auth;

namespace Application.Common.Interfaces.Repositories;

public interface IServiceRepository : IRepository<Service, int>
{
    Task<PaginatedList<ServiceDto>> GetByMenuIdAsync(int menuId, PagingOption pagingOption, CancellationToken cancellationToken);
    Task<PaginatedList<ServiceDto>> GetByRoleIdAsync(int roleId, PagingOption pagingOption, CancellationToken cancellationToken);
}