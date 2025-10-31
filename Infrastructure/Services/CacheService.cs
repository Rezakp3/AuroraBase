using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Application.Common.Interfaces.Services.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

/// <summary>
/// پیاده‌سازی سرویس مدیریت کش بر پایه IDistributedCache (Memory Cache)
/// </summary>
public class CacheService(
    IDistributedCache distributedCache,
    IMemoryCache memoryCache,
    ILogger<CacheService> logger) : ICacheService
{

    // ذخیره لیست کلیدها برای قابلیت‌های پیشرفته
    private static readonly ConcurrentDictionary<string, byte> _cacheKeys = new();

    /// <summary>
    /// دریافت مقدار از کش
    /// </summary>
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var bytes = await distributedCache.GetAsync(key, cancellationToken);
            
            if (bytes == null || bytes.Length == 0)
                return default;

            var json = Encoding.UTF8.GetString(bytes);
            return JsonSerializer.Deserialize<T>(json);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving cache for key: {Key}", key);
            return default;
        }
    }

    /// <summary>
    /// ذخیره مقدار در کش
    /// </summary>
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var options = new CacheOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(10)
        };

        await SetAsync(key, value, options, cancellationToken);
    }

    /// <summary>
    /// ذخیره مقدار با گزینه‌های کامل
    /// </summary>
    public async Task SetAsync<T>(string key, T value, CacheOptions options, CancellationToken cancellationToken = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(value);
            var bytes = Encoding.UTF8.GetBytes(json);

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = options.AbsoluteExpiration,
                AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow,
                SlidingExpiration = options.SlidingExpiration
            };

            await distributedCache.SetAsync(key, bytes, cacheOptions, cancellationToken);
            
            // ثبت کلید برای عملیات‌های پیشرفته
            _cacheKeys.TryAdd(key, 0);

            // تنظیم Callback برای حذف از لیست کلیدها
            memoryCache.Set(
                $"_callback_{key}", 
                true,
                new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = options.AbsoluteExpiration,
                    AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow,
                    SlidingExpiration = options.SlidingExpiration,
                    PostEvictionCallbacks =
                    {
                        new PostEvictionCallbackRegistration
                        {
                            EvictionCallback = (k, v, reason, state) =>
                            {
                                _cacheKeys.TryRemove(key, out _);
                            }
                        }
                    }
                });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error setting cache for key: {Key}", key);
        }
    }

    /// <summary>
    /// دریافت یا ذخیره (Get or Set Pattern)
    /// </summary>
    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var cached = await GetAsync<T>(key, cancellationToken);
        
        if (cached != null)
            return cached;

        var value = await factory();
        
        if (value != null)
            await SetAsync(key, value, expiration, cancellationToken);

        return value;
    }

    /// <summary>
    /// حذف یک کلید از کش
    /// </summary>
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await distributedCache.RemoveAsync(key, cancellationToken);
            _cacheKeys.TryRemove(key, out _);
            memoryCache.Remove($"_callback_{key}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error removing cache for key: {Key}", key);
        }
    }

    /// <summary>
    /// حذف چندین کلید با پترن (از لیست کلیدهای ذخیره شده)
    /// </summary>
    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        try
        {
            // تبدیل wildcard pattern به regex
            var regexPattern = "^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$";
            var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);

            var keysToRemove = _cacheKeys.Keys
                .Where(key => regex.IsMatch(key))
                .ToList();

            foreach (var key in keysToRemove)
            {
                await RemoveAsync(key, cancellationToken);
            }

            if (keysToRemove.Any())
            {
                logger.LogInformation("Removed {Count} keys matching pattern: {Pattern}", keysToRemove.Count, pattern);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error removing cache by pattern: {Pattern}", pattern);
        }
    }

    /// <summary>
    /// بررسی وجود یک کلید
    /// </summary>
    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var bytes = await distributedCache.GetAsync(key, cancellationToken);
            return bytes != null && bytes.Length > 0;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking cache existence for key: {Key}", key);
            return false;
        }
    }

    /// <summary>
    /// تمدید عمر کش
    /// </summary>
    public async Task RefreshAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await distributedCache.RefreshAsync(key, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error refreshing cache for key: {Key}", key);
        }
    }

    /// <summary>
    /// پاک کردن کل کش
    /// </summary>
    public Task ClearAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var keys = _cacheKeys.Keys.ToList();
            
            foreach (var key in keys)
            {
                distributedCache.Remove(key);
                memoryCache.Remove($"_callback_{key}");
            }
            
            _cacheKeys.Clear();
            
            logger.LogWarning("All cache has been cleared ({Count} keys)", keys.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error clearing cache");
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// دریافت چندین مقدار به صورت همزمان
    /// </summary>
    public async Task<IDictionary<string, T?>> GetManyAsync<T>(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        var result = new Dictionary<string, T?>();
        
        var tasks = keys.Select(async key =>
        {
            var value = await GetAsync<T>(key, cancellationToken);
            return new KeyValuePair<string, T?>(key, value);
        });

        var items = await Task.WhenAll(tasks);
        
        foreach (var item in items)
        {
            result[item.Key] = item.Value;
        }

        return result;
    }

    /// <summary>
    /// ذخیره چندین مقدار به صورت همزمان
    /// </summary>
    public async Task SetManyAsync<T>(IDictionary<string, T> items, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var tasks = items.Select(item => SetAsync(item.Key, item.Value, expiration, cancellationToken));
        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// دریافت تمام کلیدهای موجود در کش
    /// </summary>
    public Task<IEnumerable<string>> GetAllKeysAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<string>>(_cacheKeys.Keys.ToList());
    }
}