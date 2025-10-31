using System.Collections.Concurrent;
using System.Text.Json;
using Application.Common.Interfaces.Repositories;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class SettingRepository(MyContext context) : Repository<Setting, int>(context), ISettingRepository
{
    // ✅ Cache: Group → Dictionary<Key, Setting>
    private static readonly ConcurrentDictionary<string, Dictionary<string, Setting>> _settingsCache = new();
    
    // ✅ Lock برای Warm-up
    private static readonly SemaphoreSlim _warmupLock = new(1, 1);
    private static bool _isWarmedUp = false;

    #region Cache Management

    /// <summary>
    /// بارگذاری اولیه تمام تنظیمات در Cache (فراخوانی در Startup)
    /// </summary>
    public async Task WarmupCacheAsync(CancellationToken cancellationToken = default)
    {
        // اگه قبلاً Warmup شده، دیگه نیازی نیست
        if (_isWarmedUp) return;

        await _warmupLock.WaitAsync(cancellationToken);
        try
        {
            // Double-check locking
            if (_isWarmedUp) return;

            var allSettings = await _dbSet.AsNoTracking().ToListAsync(cancellationToken);

            // گروه‌بندی بر اساس Group
            var grouped = allSettings.GroupBy(s => s.Group);

            foreach (var group in grouped)
            {
                var groupDict = group.ToDictionary(s => s.Key, s => s);
                _settingsCache[group.Key] = groupDict;
            }

            _isWarmedUp = true;
        }
        finally
        {
            _warmupLock.Release();
        }
    }

    /// <summary>
    /// پاک کردن کل Cache
    /// </summary>
    public void ClearCache()
    {
        _settingsCache.Clear();
        _isWarmedUp = false;
    }

    #endregion

    #region Query Methods با Cache

    /// <summary>
    /// دریافت همه تنظیمات یک گروه خاص (با Cache)
    /// </summary>
    public async Task<List<Setting>> GetByGroupAsync(string group = "General", CancellationToken cancellationToken = default)
    {
        // چک Cache
        if (_settingsCache.TryGetValue(group, out var cachedGroup))
        {
            return cachedGroup.Values.ToList();
        }

        // اگر توی Cache نبود، از DB بخون
        var settings = await _dbSet
            .Where(s => s.Group == group)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        // اضافه به Cache
        if (settings.Count > 0)
        {
            var groupDict = settings.ToDictionary(s => s.Key, s => s);
            _settingsCache[group] = groupDict;
        }

        return settings;
    }

    /// <summary>
    /// دریافت همه تنظیمات یک گروه خاص به صورت آبجکت strongly-typed (با Cache)
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
    /// دریافت یک تنظیم خاص با گروه و کلید (با Cache)
    /// </summary>
    public async Task<Setting?> GetByKeyAsync(string key, string group = "General", CancellationToken cancellationToken = default)
    {
        // چک Cache
        if (_settingsCache.TryGetValue(group, out var cachedGroup))
        {
            return cachedGroup.TryGetValue(key, out var setting) ? setting : null;
        }

        // Fallback به DB
        var result = await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Group == group && s.Key == key, cancellationToken);

        // اگر پیدا شد، کل گروه رو Cache کن
        if (result != null)
        {
            await GetByGroupAsync(group, cancellationToken); // Load کل گروه
        }

        return result;
    }

    /// <summary>
    /// دریافت مقدار یک تنظیم خاص (فقط Value به صورت string)
    /// </summary>
    public async Task<string?> GetValueAsync(string key, string group = "General", CancellationToken cancellationToken = default)
    {
        var setting = await GetByKeyAsync(key, group, cancellationToken);
        return setting?.Value;
    }

    /// <summary>
    /// دریافت مقدار با تبدیل نوع - در صورت عدم تبدیل Exception میدهد
    /// </summary>
    public async Task<T?> GetValueAsync<T>(string key, string group = "General", CancellationToken cancellationToken = default)
    {
        var value = await GetValueAsync(key, group, cancellationToken);

        if (value == null)
            return default;

        try
        {
            return ConvertToType<T>(value);
        }
        catch (Exception ex)
        {
            throw new InvalidCastException($"Cannot convert value '{value}' to type {typeof(T).Name} for key '{key}' in group '{group}'", ex);
        }
    }

    /// <summary>
    /// دریافت مقدار با تبدیل نوع - در صورت عدم تبدیل مقدار default نوع را برمیگرداند
    /// </summary>
    public async Task<T?> GetValueOrDefaultAsync<T>(string key, string group = "General", CancellationToken cancellationToken = default)
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
    public async Task<T?> GetValueOrDefaultAsync<T>(string key, T? defaultValue, string group = "General", CancellationToken cancellationToken = default)
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

    #region Command Methods با Cache Update

    /// <summary>
    /// به‌روزرسانی مقدار یک تنظیم (با آپدیت هوشمند Cache)
    /// </summary>
    public async Task<bool> UpdateValueAsync(string key, string newValue, string group = "General", CancellationToken cancellationToken = default)
    {
        var setting = await _dbSet
            .FirstOrDefaultAsync(s => s.Group == group && s.Key == key, cancellationToken);

        if (setting == null)
            return false;

        // آپدیت در DB
        setting.Value = newValue;
        _dbSet.Update(setting);

        // ✅ آپدیت در Cache (اگر گروه Cache شده باشه)
        if (_settingsCache.TryGetValue(group, out var cachedGroup))
        {
            cachedGroup[key] = setting; // اگه وجود داشت Update، نداشت Add
        }

        return true;
    }

    /// <summary>
    /// حذف یک تنظیم (Override با حذف از Cache)
    /// </summary>
    public override void Delete(Setting entity)
    {
        base.Delete(entity);

        // ✅ حذف از Cache
        if (_settingsCache.TryGetValue(entity.Group, out var cachedGroup))
        {
            cachedGroup.Remove(entity.Key);
            
            // اگه گروه خالی شد، کل گروه رو پاک کن
            if (cachedGroup.Count == 0)
            {
                _settingsCache.TryRemove(entity.Group, out _);
            }
        }
    }

    /// <summary>
    /// حذف چندین تنظیم (Override با حذف از Cache)
    /// </summary>
    public override void DeleteRange(IEnumerable<Setting> entities)
    {
        base.DeleteRange(entities);

        // ✅ حذف از Cache
        foreach (var entity in entities)
        {
            if (_settingsCache.TryGetValue(entity.Group, out var cachedGroup))
            {
                cachedGroup.Remove(entity.Key);
                
                // اگه گروه خالی شد، کل گروه رو پاک کن
                if (cachedGroup.Count == 0)
                {
                    _settingsCache.TryRemove(entity.Group, out _);
                }
            }
        }
    }

    /// <summary>
    /// حذف بر اساس ID (Override با حذف از Cache)
    /// </summary>
    public override async Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return false;

        Delete(entity); // صدا زدن Delete که خودش Cache رو هم پاک می‌کنه
        return true;
    }

    /// <summary>
    /// افزودن تنظیم جدید (Override با اضافه به Cache)
    /// </summary>
    public override async Task<Setting> AddAsync(Setting entity, CancellationToken cancellationToken = default)
    {
        var result = await base.AddAsync(entity, cancellationToken);

        // ✅ اضافه به Cache (اگر گروه Cache شده باشه)
        if (_settingsCache.TryGetValue(entity.Group, out var cachedGroup))
        {
            cachedGroup[entity.Key] = entity;
        }

        return result;
    }

    /// <summary>
    /// افزودن چندین تنظیم (Override با اضافه به Cache)
    /// </summary>
    public override async Task AddRangeAsync(IEnumerable<Setting> entities, CancellationToken cancellationToken = default)
    {
        await base.AddRangeAsync(entities, cancellationToken);

        // ✅ اضافه به Cache
        foreach (var entity in entities)
        {
            if (_settingsCache.TryGetValue(entity.Group, out var cachedGroup))
            {
                cachedGroup[entity.Key] = entity;
            }
        }
    }

    /// <summary>
    /// بروزرسانی تنظیم (Override با آپدیت Cache)
    /// </summary>
    public override void Update(Setting entity)
    {
        base.Update(entity);

        // ✅ آپدیت در Cache
        if (_settingsCache.TryGetValue(entity.Group, out var cachedGroup))
        {
            cachedGroup[entity.Key] = entity;
        }
    }

    /// <summary>
    /// بروزرسانی چندین تنظیم (Override با آپدیت Cache)
    /// </summary>
    public override void UpdateRange(IEnumerable<Setting> entities)
    {
        base.UpdateRange(entities);

        // ✅ آپدیت در Cache
        foreach (var entity in entities)
        {
            if (_settingsCache.TryGetValue(entity.Group, out var cachedGroup))
            {
                cachedGroup[entity.Key] = entity;
            }
        }
    }

    #endregion

    #region Existence Checks (بدون تغییر)

    /// <summary>
    /// چک کردن وجود یک گروه
    /// </summary>
    public async Task<bool> GroupExistsAsync(string group, CancellationToken cancellationToken = default)
    {
        // چک Cache
        if (_settingsCache.ContainsKey(group))
            return true;

        // Fallback به DB
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(s => s.Group == group, cancellationToken);
    }

    /// <summary>
    /// چک کردن وجود یک کلید در گروه خاص
    /// </summary>
    public async Task<bool> KeyExistsAsync(string key, string group = "General", CancellationToken cancellationToken = default)
    {
        // چک Cache
        if (_settingsCache.TryGetValue(group, out var cachedGroup))
        {
            return cachedGroup.ContainsKey(key);
        }

        // Fallback به DB
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(s => s.Group == group && s.Key == key, cancellationToken);
    }

    /// <summary>
    /// لیست تمام گروه‌های موجود (یونیک)
    /// </summary>
    public async Task<List<string>> GetAllGroupsAsync(CancellationToken cancellationToken = default)
    {
        // اگر Cache پر شده، از Cache بگیر
        if (_isWarmedUp && _settingsCache.Count > 0)
        {
            return _settingsCache.Keys.OrderBy(g => g).ToList();
        }

        // Fallback به DB
        return await _dbSet
            .AsNoTracking()
            .Select(s => s.Group)
            .Distinct()
            .OrderBy(g => g)
            .ToListAsync(cancellationToken);
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// تبدیل string به نوع مورد نظر
    /// </summary>
    private static T? ConvertToType<T>(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return default;

        var targetType = typeof(T);
        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        // برای انواع ساده
        if (underlyingType == typeof(string))
            return (T)(object)value;

        if (underlyingType == typeof(int))
            return (T)(object)int.Parse(value);

        if (underlyingType == typeof(long))
            return (T)(object)long.Parse(value);

        if (underlyingType == typeof(decimal))
            return (T)(object)decimal.Parse(value);

        if (underlyingType == typeof(double))
            return (T)(object)double.Parse(value);

        if (underlyingType == typeof(bool))
            return (T)(object)bool.Parse(value);

        if (underlyingType == typeof(DateTime))
            return (T)(object)DateTime.Parse(value);

        if (underlyingType == typeof(Guid))
            return (T)(object)Guid.Parse(value);

        // برای انواع پیچیده (JSON)
        return JsonSerializer.Deserialize<T>(value);
    }

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
