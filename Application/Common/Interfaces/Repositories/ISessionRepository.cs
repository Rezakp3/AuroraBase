using Application.Common.Interfaces.Generals;
using Application.Common.Models.Pagination;
using Application.Features.AuthFeature.SessionManagement.Models;
using Core.Entities.Auth;

namespace Application.Common.Interfaces.Repositories;

public interface ISessionRepository : IRepository<Session, Guid>
{
    Task<Session> GetByTokenAsync(string token, CancellationToken ct = default);

    // پیدا کردن تمام رفرش توکن‌های فعال یک کاربر (برای Force Logout همه دستگاه‌ها)
    Task<List<Session>> GetActiveTokensByUserIdAsync(long userId, CancellationToken ct = default);
    Task<PaginatedList<SessionDto>> Search(SessionIm search, CancellationToken ct);
}