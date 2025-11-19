using Application.Common.Interfaces.Repositories;
using Core.Entities;
using Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Utils.Helpers;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// پیاده‌سازی Repository برای دسترسی به تنظیمات از دیتابیس (بدون Cache)
/// </summary>
public class SettingRepository(MyContext context) : Repository<Setting, int>(context), ISettingRepository
{
    #region Cache Management (No-Op in this layer)

    /// <summary>
    /// Warmup در این لایه انجام نمی‌شود (مسئولیت Decorator)
    /// </summary>
    public Task WarmupCacheAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Clear Cache در این لایه انجام نمی‌شود (مسئولیت Decorator)
    /// </summary>
    public void ClearCache()
    {
        // No-Op
    }

    #endregion

    #region Query Methods

    /// <summary>
    /// دریافت همه تنظیمات یک گروه خاص از دیتابیس
    /// </summary>
    public async Task<List<Setting>> GetByGroupAsync(string group = "General", CancellationToken cancellationToken = default)
    {
        return await dbSet
            .Where(s => s.Group == group)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// دریافت همه تنظیمات یک گروه خاص به صورت آبجکت strongly-typed
    /// </summary>
    public async Task<T> GetByGroupAsync<T>(string group = "General", CancellationToken cancellationToken = default)
    {
        var settings = await GetByGroupAsync(group, cancellationToken);

        if (settings.Count == 0)
            throw new InvalidOperationException($"No settings found for group '{group}'");

        // تبدیل List<Setting> به Dictionary<string, object>
        var dictionary = settings.ToDictionary(
            s => s.Key,
            s => ConvertValue(s.Value, s.DataType)
        );

        // Serialize و Deserialize برای تبدیل به مدل
        var json = JsonSerializer.Serialize(dictionary);
        var result = JsonSerializer.Deserialize<T>(json);

        return result == null
            ? throw new InvalidOperationException($"Failed to deserialize settings for group '{group}' to type {typeof(T).Name}")
            : result;
    }

    /// <summary>
    /// دریافت یک تنظیم خاص با گروه و کلید از دیتابیس
    /// </summary>
    public async Task<Setting> GetByKeyAsync(string key, string group = "General", CancellationToken cancellationToken = default)
    {
        return await dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Group == group && s.Key == key, cancellationToken);
    }

    /// <summary>
    /// دریافت مقدار یک تنظیم خاص (فقط Value به صورت string)
    /// </summary>
    public async Task<string> GetValueAsync(string key, string group = "General", CancellationToken cancellationToken = default)
    {
        var setting = await GetByKeyAsync(key, group, cancellationToken);
        return setting?.Value;
    }

    /// <summary>
    /// دریافت مقدار با تبدیل نوع - در صورت عدم تبدیل Exception میدهد
    /// </summary>
    public async Task<T> GetValueAsync<T>(string key, string group = "General", CancellationToken cancellationToken = default)
    {
        var value = await GetValueAsync(key, group, cancellationToken);

        if (value == null)
            return default;

        try
        {
            return value.ConvertTo<T>();
        }
        catch (Exception ex)
        {
            throw new InvalidCastException($"Cannot convert value '{value}' to type {typeof(T).Name} for key '{key}' in group '{group}'", ex);
        }
    }

    /// <summary>
    /// دریافت مقدار با تبدیل نوع - در صورت عدم تبدیل مقدار default نوع را برمیگرداند
    /// </summary>
    public async Task<T> GetValueOrDefaultAsync<T>(string key, string group = "General", CancellationToken cancellationToken = default)
    {
        try
        {
            return await GetValueAsync<T>(key, group, cancellationToken);
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// دریافت مقدار با تبدیل نوع - در صورت عدم تبدیل مقدار defaultValue را برمیگرداند
    /// </summary>
    public async Task<T> GetValueOrDefaultAsync<T>(string key, T defaultValue, string group = "General", CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await GetValueAsync<T>(key, group, cancellationToken);
            return result ?? defaultValue;
        }
        catch
        {
            return defaultValue;
        }
    }

    #endregion

    #region Command Methods

    /// <summary>
    /// به‌روزرسانی مقدار یک تنظیم در دیتابیس
    /// </summary>
    public async Task<bool> UpdateValueAsync(string key, string newValue, string group = "General", CancellationToken cancellationToken = default)
    {
        var setting = await dbSet
            .FirstOrDefaultAsync(s => s.Group == group && s.Key == key, cancellationToken);

        if (setting == null)
            return false;

        setting.Value = newValue;
        dbSet.Update(setting);

        return true;
    }

    #endregion

    #region Existence Checks

    /// <summary>
    /// چک کردن وجود یک گروه در دیتابیس
    /// </summary>
    public async Task<bool> GroupExistsAsync(string group, CancellationToken cancellationToken = default)
    {
        return await dbSet
            .AsNoTracking()
            .AnyAsync(s => s.Group == group, cancellationToken);
    }

    /// <summary>
    /// چک کردن وجود یک کلید در گروه خاص در دیتابیس
    /// </summary>
    public async Task<bool> KeyExistsAsync(string key, string group = "General", CancellationToken cancellationToken = default)
    {
        return await dbSet
            .AsNoTracking()
            .AnyAsync(s => s.Group == group && s.Key == key, cancellationToken);
    }

    /// <summary>
    /// لیست تمام گروه‌های موجود (یونیک) از دیتابیس
    /// </summary>
    public async Task<List<string>> GetAllGroupsAsync(CancellationToken cancellationToken = default)
    {
        return await dbSet
            .AsNoTracking()
            .Select(s => s.Group)
            .Distinct()
            .OrderBy(g => g)
            .ToListAsync(cancellationToken);
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// تبدیل Value به نوع مناسب بر اساس DataType
    /// </summary>
    private static object ConvertValue(string value, Core.Enums.ESettingDataType dataType) => dataType switch
    {
        Core.Enums.ESettingDataType.Int => int.Parse(value),
        Core.Enums.ESettingDataType.Bool => bool.Parse(value),
        Core.Enums.ESettingDataType.Decimal => decimal.Parse(value),
        Core.Enums.ESettingDataType.Json => JsonSerializer.Deserialize<object>(value) ?? value,
        _ => value // String, Email, Url, Password
    };

    #endregion
}

