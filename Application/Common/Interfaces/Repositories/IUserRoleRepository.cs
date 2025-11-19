using Application.Common.Interfaces.Generals;
using Core.Entities.Auth.Relation;

namespace Application.Common.Interfaces.Repositories;

public interface IUserRoleRepository : IRepository<UserRole, int> 
{
    // متد حیاتی: گرفتن لیست ID نقش‌های یک کاربر
    Task<List<int>> GetUserRoleIdsAsync(long userId, CancellationToken ct = default);

    // مدیریت انتساب نقش به کاربر
    Task AssignRoleToUserAsync(long userId, int roleId, CancellationToken ct = default);
    Task RemoveRoleFromUserAsync(long userId, int roleId, CancellationToken ct = default);
}