using Aurora.Cache.Models;

namespace Aurora.Cache.Services;

/// <summary>
/// سرویس مدیریت کش با قابلیت‌های پیشرفته
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// دریافت مقدار از کش با تبدیل خودکار
    /// </summary>
    Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// ذخیره مقدار در کش
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// ذخیره مقدار با گزینه‌های کامل
    /// </summary>
    Task SetAsync<T>(string key, T value, CacheOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت یا ذخیره (Get or Set Pattern)
    /// </summary>
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// حذف یک کلید از کش
    /// </summary>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// حذف چندین کلید با پترن
    /// </summary>
    Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);

    /// <summary>
    /// بررسی وجود یک کلید
    /// </summary>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// تمدید عمر کش (Refresh)
    /// </summary>
    Task RefreshAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// پاک کردن کل کش
    /// </summary>
    Task ClearAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت چندین مقدار به صورت همزمان
    /// </summary>
    Task<IDictionary<string, T>> GetManyAsync<T>(IEnumerable<string> keys, CancellationToken cancellationToken = default);

    /// <summary>
    /// ذخیره چندین مقدار به صورت همزمان
    /// </summary>
    Task SetManyAsync<T>(IDictionary<string, T> items, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت تمام کلیدهای موجود در کش
    /// </summary>
    Task<IEnumerable<string>> GetAllKeysAsync(CancellationToken cancellationToken = default);
}
