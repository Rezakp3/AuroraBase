using Application.Common.Interfaces.Generals;
using Application.Common.Models.Pagination;
using Application.Features.RoleFeatures.RoleClaimFeatures.Models;
using Application.Features.RoleFeatures.RoleManagement.Models;
using Core.Entities.Auth;

namespace Application.Common.Interfaces.Repositories;

public interface IRoleRepository : IRepository<Role, int>
{
    Task AssignMenuesToRole(int roleId, IEnumerable<int> menuIds, CancellationToken ct);
    Task<PaginatedList<RoleDto>> Search(RoleIm query, CancellationToken ct);
    void AddClaims(int roleId, IEnumerable<RoleClaimDto> claims, CancellationToken cancellationToken);
    void DeleteRoleClaims(IEnumerable<int> ids);
    Task<RoleClaim> GetClaimByIdAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<RoleClaimDto>> GetClaimsByRoleIdAsync(int roleId, CancellationToken cancellationToken);
}