using Application.Common.Interfaces.Generals;
using Core.Entities.Auth;

namespace Application.Common.Interfaces.Repositories;

public interface IUserRepository : IRepository<User, long>
{
    Task<User?> GetByUserNameAsync(string username, CancellationToken ct = default);
    Task<User?> GetByIdForAuthAsync(long id, CancellationToken ct = default);
}