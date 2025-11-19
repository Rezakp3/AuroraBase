using Application.Common.Interfaces.Repositories;
using Aurora.Cache.Models;
using Aurora.Cache.Services;
using Core.Entities.Auth.Relation;
using Infrastructure.Persistence.Repositories.Base;

namespace Infrastructure.Persistence.Repositories.Cached;

public class CachedRoleServiceRepository(
    IRoleServiceRepository _innerRepo,
    ICacheService _cache
    ) : CachedRepositoryBase<RoleService, int>(_innerRepo, _cache), IRoleServiceRepository
{
    protected override string CachePrefix => "role_services";

    protected override async Task InvalidateRelatedCacheAsync(RoleService entity, CancellationToken ct)
    {
        await _cache.RemoveAsync(GetIdKey(entity.Id), ct);

        // باطل کردن کش دسترسی‌های این نقش
        var permsKey = GetRolePermsKey(entity.RoleId);
        await _cache.RemoveAsync(permsKey, ct);
    }

    private static string GetRolePermsKey(int roleId) => $"auth:role:{roleId}:perms";

    // --- پیاده‌سازی متدهای اختصاصی ---

    public async Task<HashSet<string>> GetPermittedIdentifiersAsync(int roleId, CancellationToken ct = default)
    {
        var key = GetRolePermsKey(roleId);

        return await _cache.GetOrSetAsync(
            key,
            () => _innerRepo.GetPermittedIdentifiersAsync(roleId, ct),
            CacheOptions.Long.AbsoluteExpirationRelativeToNow, // تغییرات کم است
            ct
        );
    }

    public async Task SyncPermissionsAsync(int roleId, IEnumerable<int> serviceIds, CancellationToken ct = default)
    {
        // چون تعداد تغییرات زیاد است (حذف همه، اضافه همه)، بهتر است مستقیم به inner پاس دهیم
        await _innerRepo.SyncPermissionsAsync(roleId, serviceIds, ct);

        // و در نهایت یکبار کش را پاک کنیم
        await _cache.RemoveAsync(GetRolePermsKey(roleId), ct);
    }
}