using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Aurora.Cache.Models;
using Aurora.Cache.Enums;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace Aurora.Cache.Services;

/// <summary>
/// پیاده‌سازی مدرن سرویس کش با استفاده از HybridCache (.NET 9+)
/// تضمین هماهنگی در محیط توزیع شده (Distributed)
/// </summary>
public class CacheService(
    HybridCache hybridCache,
    ILogger<CacheService> logger) : ICacheService
{
    // کلید مخصوص برای ذخیره لیست تمام کلیدهای سیستم (برای GetAll و Pattern)
    private const string KeyRegistryKey = "sys:key_registry";

    /// <summary>
    /// دریافت یا ذخیره (بهینه شده با Stampede Protection)
    /// </summary>
    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var options = new HybridCacheEntryOptions
        {
            Expiration = expiration ?? TimeSpan.FromMinutes(10),
            LocalCacheExpiration = TimeSpan.FromMinutes(1) // کش رم زودتر آپدیت شود
        };

        // استخراج تگ از کلید برای مدیریت گروهی (مثلا: "User:10" -> Tag: "User")
        var tags = ExtractTags(key);

        var result = await hybridCache.GetOrCreateAsync(key, async token =>
        {
            var value = await factory();
            // اگر مقدار null نبود، کلید را در رجیستری ثبت کن
            if (value is not null)
            {
                await RegisterKeyAsync(key, token);
            }
            return value;
        }, options, tags, cancellationToken);

        return result;
    }

    /// <summary>
    /// ذخیره مقدار
    /// </summary>
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        var options = new CacheOptions { AbsoluteExpirationRelativeToNow = expiration };
        await SetAsync(key, value, options, cancellationToken);
    }

    /// <summary>
    /// ذخیره با آپشن‌های کامل
    /// </summary>
    public async Task SetAsync<T>(string key, T value, CacheOptions options, CancellationToken cancellationToken = default)
    {
        try
        {
            var hybridOptions = new HybridCacheEntryOptions
            {
                Expiration = options.AbsoluteExpirationRelativeToNow ?? TimeSpan.FromMinutes(10),
                LocalCacheExpiration = TimeSpan.FromMinutes(1)
            };

            var tags = ExtractTags(key);

            await hybridCache.SetAsync(key, value, hybridOptions, tags, cancellationToken);
            await RegisterKeyAsync(key, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error setting cache for key: {Key}", key);
        }
    }

    /// <summary>
    /// دریافت مقدار
    /// </summary>
    public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        // HybridCache متد Get خالی ندارد، از GetOrCreate با مقدار پیش‌فرض استفاده می‌کنیم
        // در .NET 10 ممکن است متد TryGetValue اضافه شده باشد
        return await hybridCache.GetOrCreateAsync<T>(
            key,
            _ => ValueTask.FromResult<T>(default!),
            cancellationToken: cancellationToken
        );
    }

    /// <summary>
    /// حذف آیتم
    /// </summary>
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await hybridCache.RemoveAsync(key, cancellationToken);
        await UnregisterKeyAsync(key, cancellationToken);
    }

    /// <summary>
    /// حذف با پترن (هوشمند)
    /// </summary>
    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        // 1. اگر پترن ساده است (مثلاً "User:*")، از تگ استفاده کن (بسیار سریع)
        if (pattern.EndsWith("*") && !pattern.Contains(".*"))
        {
            var tag = pattern.TrimEnd('*', ':');
            logger.LogInformation("Optimized RemoveByPattern used Tag: {Tag}", tag);
            await hybridCache.RemoveByTagAsync(tag, cancellationToken);

            // حذف از رجیستری (غیر بهینه ولی لازم برای هماهنگی GetAll)
            // نکته: در محیط پروداکشن با لود بالا، ممکن است بیخیال آپدیت دقیق رجیستری شوید
            var allKeys = await GetRegistryAsync(cancellationToken);
            var prefix = pattern.TrimEnd('*');
            foreach (var k in allKeys.Where(k => k.StartsWith(prefix)))
            {
                await UnregisterKeyAsync(k, cancellationToken);
            }
            return;
        }

        // 2. اگر پترن Regex پیچیده است، باید تمام کلیدها را چک کنیم (کند)
        var regex = new Regex("^" + Regex.Escape(pattern).Replace("\\*", ".*") + "$", RegexOptions.IgnoreCase);
        var keys = await GetRegistryAsync(cancellationToken);

        var matches = keys.Where(k => regex.IsMatch(k)).ToList();
        foreach (var key in matches)
        {
            await RemoveAsync(key, cancellationToken);
        }
    }

    /// <summary>
    /// دریافت چندین آیتم
    /// </summary>
    public async Task<IDictionary<string, T>> GetManyAsync<T>(IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        var result = new Dictionary<string, T>();
        // HybridCache هنوز GetMany ندارد، باید موازی اجرا کنیم
        await Parallel.ForEachAsync(keys, cancellationToken, async (key, ct) =>
        {
            var value = await GetAsync<T>(key, ct);
            if (value != null)
            {
                lock (result)
                {
                    result[key] = value;
                }
            }
        });
        return result;
    }

    /// <summary>
    /// ذخیره چندین آیتم
    /// </summary>
    public async Task SetManyAsync<T>(IDictionary<string, T> items, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        await Parallel.ForEachAsync(items, cancellationToken, async (item, ct) =>
        {
            await SetAsync(item.Key, item.Value, expiration, ct);
        });
    }

    /// <summary>
    /// دریافت همه کلیدها (از رجیستری توزیع شده)
    /// </summary>
    public async Task<IEnumerable<string>> GetAllKeysAsync(CancellationToken cancellationToken = default)
    {
        return await GetRegistryAsync(cancellationToken);
    }

    /// <summary>
    /// وجود کلید
    /// </summary>
    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        // راه حل موقت: تلاش برای دریافت. اگر null نبود یعنی هست.
        var val = await GetAsync<bool>(key, cancellationToken);
        return val is true;
    }

    public async Task RefreshAsync(string key, CancellationToken cancellationToken = default)
    {
        // HybridCache مدیریت Refresh را داخلی انجام می‌دهد، اما می‌توانیم یک Set مجدد بزنیم
        var val = await GetAsync<object>(key, cancellationToken);
        if (val != null)
        {
            // بازنشانی تایمر با خواندن و نوشتن مجدد (کمی پرهزینه)
            // در نسخه نهایی ممکن است متد Refresh اضافه شود
            await SetAsync(key, val, cancellationToken: cancellationToken);
        }
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        var keys = await GetRegistryAsync(cancellationToken);
        foreach (var key in keys)
        {
            await hybridCache.RemoveAsync(key, cancellationToken);
        }
        // پاک کردن رجیستری
        await hybridCache.RemoveAsync(KeyRegistryKey, cancellationToken);
    }

    #region Private Helpers (Distributed Key Registry)

    // استخراج تگ از کلید (مثلا "Auth:User:1" -> Tag "Auth" و "Auth:User")
    private static IEnumerable<string> ExtractTags(string key)
    {
        var parts = key.Split(':');
        if (parts.Length > 1)
        {
            // تگ اول: گروپ اصلی (مثلا Settings)
            yield return parts[0];
            // تگ دوم: زیرگروه (مثلا Settings:Smtp)
            if (parts.Length > 2) yield return $"{parts[0]}:{parts[1]}";
        }
    }

    private async Task RegisterKeyAsync(string key, CancellationToken ct)
    {
        // نکته مهم: این بخش می‌تواند گلوگاه باشد. برای پروژه‌های بسیار بزرگ
        // باید از Redis Set مستقیم استفاده کرد. اینجا برای سازگاری با HybridCache
        // از مدل Read-Modify-Write استفاده می‌کنیم که ریسک Race Condition دارد
        // اما برای "زیرساخت ادمین" قابل قبول است.

        var registry = await GetRegistryAsync(ct);
        if (!registry.Contains(key))
        {
            registry.Add(key);
            await SaveRegistryAsync(registry, ct);
        }
    }

    private async Task UnregisterKeyAsync(string key, CancellationToken ct)
    {
        var registry = await GetRegistryAsync(ct);
        if (registry.Contains(key))
        {
            registry.Remove(key);
            await SaveRegistryAsync(registry, ct);
        }
    }

    private async Task<HashSet<string>> GetRegistryAsync(CancellationToken ct)
    {
        return await hybridCache.GetOrCreateAsync(
            KeyRegistryKey,
            _ => ValueTask.FromResult(new HashSet<string>()),
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromHours(24) }, // رجیستری همیشه باید زنده بماند
            cancellationToken: ct
        );
    }

    private async Task SaveRegistryAsync(HashSet<string> registry, CancellationToken ct)
    {
        // ذخیره رجیستری با زمان طولانی
        await hybridCache.SetAsync(
            KeyRegistryKey,
            registry,
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromHours(24) },
            cancellationToken: ct
        );
    }

    #endregion
}