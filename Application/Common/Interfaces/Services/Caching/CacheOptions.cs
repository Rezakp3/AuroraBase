using Core.Enums;

namespace Application.Common.Interfaces.Services.Caching;

/// <summary>
/// گزینه‌های کش
/// </summary>
public class CacheOptions
{
    /// <summary>
    /// زمان انقضای مطلق (Absolute Expiration)
    /// </summary>
    public DateTimeOffset? AbsoluteExpiration { get; set; }

    /// <summary>
    /// زمان انقضای نسبی از الان
    /// </summary>
    public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }

    /// <summary>
    /// زمان انقضای لغزشی (هر بار که دسترسی بشه، تمدید میشه)
    /// </summary>
    public TimeSpan? SlidingExpiration { get; set; }

    /// <summary>
    /// اولویت کش (برای تعیین اینکه کدوم کش زودتر پاک بشه)
    /// </summary>
    public CacheItemPriority Priority { get; set; } = CacheItemPriority.Normal;

    /// <summary>
    /// ایجاد گزینه‌های پیش‌فرض (10 دقیقه)
    /// </summary>
    public static CacheOptions Default => new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
        Priority = CacheItemPriority.Normal
    };

    /// <summary>
    /// کش کوتاه‌مدت (1 دقیقه)
    /// </summary>
    public static CacheOptions Short => new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1),
        Priority = CacheItemPriority.Low
    };

    /// <summary>
    /// کش میان‌مدت (30 دقیقه)
    /// </summary>
    public static CacheOptions Medium => new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
        Priority = CacheItemPriority.Normal
    };

    /// <summary>
    /// کش بلندمدت (1 ساعت)
    /// </summary>
    public static CacheOptions Long => new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
        Priority = CacheItemPriority.High
    };

    /// <summary>
    /// کش دائمی (تا زمانی که به صورت دستی حذف نشه)
    /// </summary>
    public static CacheOptions Permanent => new()
    {
        Priority = CacheItemPriority.NeverRemove
    };
}
