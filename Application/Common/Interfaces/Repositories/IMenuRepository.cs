using Application.Common.Interfaces.Generals;
using Core.Entities.Auth;

namespace Application.Common.Interfaces.Repositories;

public interface IMenuRepository : IRepository<Menu, int>
{
    Task<IEnumerable<Menu>> GetByRoleId(int roleId,CancellationToken ct = default);
}