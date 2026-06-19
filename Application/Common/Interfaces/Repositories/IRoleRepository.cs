using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Application.Common.Models.Pagination;
using Application.Features.RoleFeatures.RoleClaimFeatures.Models;
using Application.Features.RoleFeatures.RoleManagement.Models;
using Core.Entities.Auth;

namespace Application.Common.Interfaces.Repositories;

public interface IRoleRepository : IRepository<Role, int>
{
    Task<PaginatedList<RoleDto>> Search(RoleIm query, CancellationToken ct);
    Task<RoleClaim> GetClaimByIdAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<RoleClaimDto>> GetClaimsByRoleIdAsync(int roleId, CancellationToken cancellationToken);
    Task<CursorPaginatedList<BaseDropDown<int>, int>> DropDown(string search, CursorPagingOption<int> pagingOption, CancellationToken ct);
    Task<IEnumerable<BaseDropDown<int>>> ServiceDropDown(int roleId, string search, CancellationToken cancellationToken);
    Task<IEnumerable<BaseDropDown<int>>> MenuDropDown(int roleId, string search, CancellationToken cancellationToken);
    
    Task DeleteMenues(int roleId, CancellationToken ct);
    Task AddRangeMenues(int roleId, IEnumerable<int> menuIds, CancellationToken ct);
    void AddClaim(RoleClaim claim, CancellationToken cancellationToken);
    void DeleteClaim(RoleClaim claim);
}