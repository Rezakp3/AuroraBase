using Application.Common.Interfaces.Generals;
using Application.Common.Models.Pagination;
using Application.Features.MenuFeature.Models;
using Core.Entities.Auth;

namespace Application.Common.Interfaces.Repositories;

public interface IMenuRepository : IRepository<Menu, int>
{
    Task<IEnumerable<Menu>> GetByRoleId(int roleId,CancellationToken ct = default);
    Task<IEnumerable<Menu>> GetByUserId(long userId, CancellationToken cancellationToken);
    Task<PaginatedList<MenuDto>> SearchAsync(SearchMenuDto searchMenuDto, CancellationToken cancellationToken);
}