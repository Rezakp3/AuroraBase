using Application.Common.Interfaces.Repositories;
using Aurora.Cache.Models;
using Aurora.Cache.Services;
using Core.Entities.Auth.Relation;
using Infrastructure.Persistence.Repositories.Base;

namespace Infrastructure.Persistence.Repositories.Cached;

public class CachedUserRoleRepository(
    IUserRoleRepository _innerRepo,
    ICacheService _cache
    ) : CachedRepositoryBase<UserRole, int>(_innerRepo, _cache), IUserRoleRepository
{
    protected override string CachePrefix => "user_roles";

    // این متد حیاتی است: وقتی یک نقش به کاربر داده می‌شه یا گرفته می‌شه، اینجا اجرا میشه
    protected override async Task InvalidateRelatedCacheAsync(UserRole entity, CancellationToken ct)
    {
        // 1. پاک کردن کش خود این رکورد (توسط Base هم میتونست انجام شه ولی اینجا دستی هم بد نیست)
        await _cache.RemoveAsync(GetIdKey(entity.Id), ct);

        // 2. مهمترین بخش: پاک کردن کش لیست نقش‌های این کاربر
        // کلید مثال: auth:user:150:roles
        var userRolesKey = GetUserRolesKey(entity.UserId);
        await _cache.RemoveAsync(userRolesKey, ct);
    }

    // متد کمکی برای ساخت کلید لیست
    private static string GetUserRolesKey(long userId) => $"auth:user:{userId}:roles";

    // --- پیاده‌سازی متدهای اختصاصی اینترفیس ---

    public async Task<List<int>> GetUserRoleIdsAsync(long userId, CancellationToken ct = default)
    {
        var key = GetUserRolesKey(userId);

        // کش کردن لیست نقش‌های یک کاربر
        return await _cache.GetOrSetAsync(
            key,
            () => _innerRepo.GetUserRoleIdsAsync(userId, ct),
            CacheOptions.Medium.AbsoluteExpirationRelativeToNow, // مثلا 30 دقیقه
            ct
        );
    }

    public async Task AssignRoleToUserAsync(long userId, int roleId, CancellationToken ct = default)
    {
        // استفاده از متد Add کلاس پدر برای بهره‌مندی از Invalidate خودکار
        await AddAsync(new UserRole { UserId = userId, RoleId = roleId }, ct);
    }

    public async Task RemoveRoleFromUserAsync(long userId, int roleId, CancellationToken ct = default)
    {
        // چون متد Delete کلاس پدر یک Entity کامل می‌گیره، ما باید اول پیداش کنیم
        // نکته: اینجا ممکنه یک کوئری اضافه زده بشه، ولی استانداردتره.
        // اگر پرفورمنس حیاتیه، باید دستی پیاده کنید.

        // راه حل دستی و سریع‌تر بدون Load کردن:
        await _innerRepo.RemoveRoleFromUserAsync(userId, roleId, ct);
        // فقط باید دستی کش رو پاک کنیم
        await _cache.RemoveAsync(GetUserRolesKey(userId), ct);
    }
}