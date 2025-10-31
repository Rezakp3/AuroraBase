using Application.Common.Interfaces.Generals;
using Core.Entities;

namespace Application.Common.Interfaces.Repositories;

public interface ISettingRepository : IRepository<Setting, int>
{
    /// <summary>
    /// دریافت همه تنظیمات یک گروه خاص (پیش‌فرض: General)
    /// </summary>
    Task<List<Setting>> GetByGroupAsync(string group = "General", CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت همه تنظیمات یک گروه خاص به صورت آبجکت (پیش‌فرض: General)
    /// </summary>
    Task<T> GetByGroupAsync<T>(string group = "General", CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت یک تنظیم خاص با کلید (اگر گروه مشخص نشه از General استفاده می‌شه)
    /// </summary>
    Task<Setting?> GetByKeyAsync(string key, string group = "General", CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت مقدار یک تنظیم خاص (فقط Value) - پیش‌فرض: General
    /// </summary>
    Task<string?> GetValueAsync(string key, string group = "General", CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت مقدار یه تنظیم خاص همراه با کست به تایپ مورد نظر (پیش‌فرض: General)
    /// ** اگر تایپ مورد نظر قابل تبدیل نباشه اکسپشن میده
    /// </summary>
    Task<T?> GetValueAsync<T>(string key, string group = "General", CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت مقدار یه تنظیم خاص همراه با کست به تایپ مورد نظر (پیش‌فرض: General)
    /// ** اگر تایپ مورد نظر قابل تبدیل نباشه مقدار دیفالت اون تایپ رو برمیگردونه
    /// </summary>
    Task<T?> GetValueOrDefaultAsync<T>(string key, string group = "General", CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت مقدار یه تنظیم خاص همراه با کست به تایپ مورد نظر (پیش‌فرض: General)
    /// ** اگر تایپ مورد نظر قابل تبدیل نباشه مقدار دیفالت ارسالی رو برمیگردونه
    /// </summary>
    Task<T?> GetValueOrDefaultAsync<T>(string key, T? defaultValue, string group = "General", CancellationToken cancellationToken = default);

    /// <summary>
    /// به‌روزرسانی مقدار یک تنظیم (پیش‌فرض: General)
    /// </summary>
    Task<bool> UpdateValueAsync(string key, string newValue, string group = "General", CancellationToken cancellationToken = default);

    /// <summary>
    /// چک کردن وجود یک گروه
    /// </summary>
    Task<bool> GroupExistsAsync(string group, CancellationToken cancellationToken = default);

    /// <summary>
    /// چک کردن وجود یک کلید در گروه خاص (پیش‌فرض: General)
    /// </summary>
    Task<bool> KeyExistsAsync(string key, string group = "General", CancellationToken cancellationToken = default);

    /// <summary>
    /// لیست تمام گروه‌های موجود (یونیک)
    /// </summary>
    Task<List<string>> GetAllGroupsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// بارگذاری اولیه تمام تنظیمات در Cache (فراخوانی در Startup)
    /// </summary>
    Task WarmupCacheAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// پاک کردن کل Cache
    /// </summary>
    void ClearCache();
}