using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Application.Common.Models.Pagination;
using Application.Features.MenuFeature.MenuManagement.Models;
using Core.Entities.Auth;

namespace Application.Common.Interfaces.Repositories;

public interface IMenuRepository : IRepository<Menu, int>
{
    Task<IEnumerable<Menu>> GetByRoleId(int roleId, CancellationToken ct = default);
    Task<IEnumerable<Menu>> GetByUserId(long userId, CancellationToken cancellationToken);
    Task<List<MenuDto>> GetMenusByRoleIds(IEnumerable<int> userRoleIds, CancellationToken ct);
    Task<PaginatedList<MenuDto>> SearchAsync(SearchMenuDto searchMenuDto, CancellationToken cancellationToken);
    Task<CursorPaginatedList<BaseDropDown<int>, int>> DropDown(string search, CursorPagingOption<int> pagingOption, CancellationToken ct);
    Task DeleteMenuServicesByMenuId(int menuId, CancellationToken ct);
    Task AddRangeServices(int menuId, IEnumerable<int> serviceIds, CancellationToken ct);
    Task DeleteMenuRolesByMenuId(int menuId, CancellationToken ct);
    Task AddRangeRoles(int menuId, IEnumerable<int> roleIds, CancellationToken ct);
    Task<IEnumerable<BaseDropDown<int>>> MenuRolesDropDown(int menuId, string roleTitle, CancellationToken ct);
    Task<IEnumerable<BaseDropDown<int>>> MenuServicesDropDown(int menuId, string serviceName, CancellationToken ct);
}