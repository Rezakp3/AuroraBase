using System.Text.Json;
using Application.Common.Interfaces.Repositories;
using Aurora.Cache.Helpers;
using Aurora.Cache.Models;
using Aurora.Cache.Services;
using Core.Entities;
using Core.Enums;
using Infrastructure.Persistence.Repositories.Base;

namespace Infrastructure.Persistence.Repositories.Cached;

public class CachedSettingRepository(
    ISettingRepository _innerRepo,
    ICacheService _cache
    ) : CachedRepositoryBase<Setting, int>(
        _innerRepo, cache: _cache), ISettingRepository
{
    // تعیین پیشوند کلیدها برای کلاس پدر
    protected override string CachePrefix => "settings";

    /// <summary>
    /// استراتژی ابطال کش مخصوص تنظیمات
    /// وقتی متدهای Add/Update/Delete در کلاس پدر صدا زده می‌شوند، این متد اجرا می‌شود
    /// </summary>
    protected override async Task InvalidateRelatedCacheAsync(Setting entity, CancellationToken ct)
    {
        // 1. پاک کردن کش خود آیتم (با ID) - توسط کلاس پدر هم انجام می‌شود اما اینجا صریح می‌گوییم
        await _cache.RemoveAsync(GetIdKey(entity.Id), ct);

        // 2. حیاتی: پاک کردن کش گروه مربوطه (چون لیست گروه تغییر کرده)
        // مثال: settings:Smtp:ALL
        var groupKey = CacheKeyBuilder.ForSetting(entity.Group, "ALL");
        await _cache.RemoveAsync(groupKey, ct);

        // 3. پاک کردن لیست گروه‌ها (چون شاید این تنظیم، یک گروه جدید ایجاد کرده باشد)
        await _cache.RemoveAsync($"{CachePrefix}:groups:list", ct);
    }

    #region Setting Specific Queries

    public async Task<List<Setting>> GetByGroupAsync(string group, CancellationToken cancellationToken = default)
    {
        var cacheKey = CacheKeyBuilder.ForSetting(group, "ALL");

        return await _cache.GetOrSetAsync(
            cacheKey,
            () => _innerRepo.GetByGroupAsync(group, cancellationToken),
            CacheOptions.Long.AbsoluteExpirationRelativeToNow, // تنظیمات دیر تغییر می‌کنند (مثلا 1 ساعت)
            cancellationToken
        );
    }

    public async Task<Setting> GetByKeyAsync(string key, string group, CancellationToken cancellationToken = default)
    {
        // بهینه‌سازی: به جای کش کردن تکی هر کلید، کل گروه را می‌گیریم
        // این کار تعداد درخواست‌های دیتابیس/کش را کاهش می‌دهد
        var groupSettings = await GetByGroupAsync(group, cancellationToken);
        return groupSettings.FirstOrDefault(s => s.Key == key);
    }

    public async Task<T> GetValueAsync<T>(string key, string group = "General", CancellationToken cancellationToken = default)
    {
        var setting = await GetByKeyAsync(key, group, cancellationToken);
        if (setting == null) return default;

        return ConvertSettingValue<T>(setting.Value, setting.DataType);
    }

    public async Task<T> GetValueOrDefaultAsync<T>(string key, string group = "General", CancellationToken cancellationToken = default)
    {
        try
        {
            var val = await GetValueAsync<T>(key, group, cancellationToken);
            // اگر T یک value type باشد (مثل int) و مقدار null برگردد (که دیفالت است)، همان دیفالت تایپ است.
            // اگر نیاز به مقدار پیش‌فرض خاصی دارید، باید logic اضافه کنید.
            return val;
        }
        catch
        {
            return default;
        }
    }

    public async Task<T> GetValueOrDefaultAsync<T>(string key, T defaultValue, string group = "General", CancellationToken cancellationToken = default)
    {
        try
        {
            var val = await GetValueAsync<T>(key, group, cancellationToken);
            // چک کردن برابری با مقدار پیش‌فرض تایپ (مثلا 0 برای int)
            if (EqualityComparer<T>.Default.Equals(val, default))
                return defaultValue;

            return val;
        }
        catch
        {
            return defaultValue;
        }
    }

    public async Task<T> GetByGroupAsync<T>(string group, CancellationToken cancellationToken = default)
    {
        var settings = await GetByGroupAsync(group, cancellationToken);
        if (settings.Count == 0) return default;

        // استفاده از Helper برای تبدیل دیکشنری به آبجکت T
        return BindSettingsToModel<T>(settings);
    }

    public async Task<List<string>> GetAllGroupsAsync(CancellationToken cancellationToken = default)
    {
        var key = $"{CachePrefix}:groups:list";
        return await _cache.GetOrSetAsync(
            key,
            () => _innerRepo.GetAllGroupsAsync(cancellationToken),
            CacheOptions.Long.AbsoluteExpirationRelativeToNow,
            cancellationToken
        );
    }

    public async Task<bool> GroupExistsAsync(string group, CancellationToken cancellationToken = default)
    {
        // اول کش لیست گروه‌ها را چک می‌کنیم (سریع‌تر)
        var groups = await GetAllGroupsAsync(cancellationToken);
        if (groups.Contains(group)) return true;

        // فال‌بک به دیتابیس (اگر نیاز بود)
        return await _innerRepo.GroupExistsAsync(group, cancellationToken);
    }

    public async Task<bool> KeyExistsAsync(string key, string group, CancellationToken cancellationToken = default)
    {
        var settings = await GetByGroupAsync(group, cancellationToken);
        return settings.Any(s => s.Key == key);
    }

    #endregion

    #region Setting Specific Commands

    public async Task<bool> UpdateValueAsync(string key, string newValue, string group, CancellationToken cancellationToken = default)
    {
        // اجرای عملیات در دیتابیس
        var result = await _innerRepo.UpdateValueAsync(key, newValue, group, cancellationToken);

        if (result)
        {
            // دستی: باید کش گروه را باطل کنیم
            // ما نمی‌توانیم از InvalidateRelatedCacheAsync استفاده کنیم چون آبجکت Setting کامل را نداریم
            // پس دستی کلید گروه را می‌سازیم و پاک می‌کنیم
            var groupKey = CacheKeyBuilder.ForSetting(group, "ALL");
            await _cache.RemoveAsync(groupKey, cancellationToken);
        }

        return result;
    }

    public async Task WarmupCacheAsync(CancellationToken cancellationToken = default)
    {
        // 1. گرفتن همه گروه‌ها
        var groups = await _innerRepo.GetAllGroupsAsync(cancellationToken);

        // 2. لود کردن هر گروه در کش
        foreach (var group in groups)
        {
            await GetByGroupAsync(group, cancellationToken);
        }
    }

    public async Task ClearCache(CancellationToken cancellationToken = default)
    {
        // استفاده از متد هوشمند RemoveByPattern سرویس کش
        await _cache.RemoveByPatternAsync("settings:*", cancellationToken);
    }

    #endregion

    #region Private Helpers

    private static T ConvertSettingValue<T>(string value, ESettingDataType type)
    {
        if (string.IsNullOrEmpty(value)) return default;

        try
        {
            var targetType = typeof(T);

            // هندل کردن نوع خاص JSON
            if (type == ESettingDataType.Json)
            {
                return JsonSerializer.Deserialize<T>(value);
            }

            // تبدیل انواع ساده
            return (T)Convert.ChangeType(value, targetType);
        }
        catch
        {
            return default;
        }
    }

    private static T BindSettingsToModel<T>(List<Setting> settings)
    {
        var instance = Activator.CreateInstance<T>();
        var props = typeof(T).GetProperties();
        var settingDict = settings.ToDictionary(s => s.Key, s => s, StringComparer.OrdinalIgnoreCase);

        foreach (var prop in props)
        {
            if (settingDict.TryGetValue(prop.Name, out var setting))
            {
                try
                {
                    // تبدیل مقدار و ست کردن روی پراپرتی مدل
                    var convertedVal = ConvertSettingValue<object>(setting.Value, setting.DataType);
                    // نکته: اینجا نیاز به تبدیل دقیق‌تر برای انواع خاص مثل Guid یا Enum داریم
                    // که برای سادگی از Convert.ChangeType استفاده کردیم
                    if (convertedVal != null)
                    {
                        prop.SetValue(instance, Convert.ChangeType(convertedVal, prop.PropertyType));
                    }
                }
                catch { /* Ignore errors for single properties */ }
            }
        }
        return instance;
    }

    #endregion
}