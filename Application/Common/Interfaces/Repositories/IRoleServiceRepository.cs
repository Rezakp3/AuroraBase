using Application.Common.Interfaces.Generals;
using Core.Entities.Auth.Relation;

namespace Application.Common.Interfaces.Repositories;

public interface IRoleServiceRepository : IRepository<RoleService, int>
{
    Task<HashSet<string>> GetPermittedIdentifiersAsync(int roleId, CancellationToken ct = default);

    Task SyncPermissionsAsync(int roleId, IEnumerable<int> serviceIds, CancellationToken ct = default);
}