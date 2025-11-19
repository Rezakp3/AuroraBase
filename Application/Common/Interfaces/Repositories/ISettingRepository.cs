using Application.Common.Interfaces.Generals;
using Core.Entities;

namespace Application.Common.Interfaces.Repositories;

/// <summary>
/// رابط Repository برای مدیریت تنظیمات سیستم با قابلیت Cache
/// </summary>
public interface ISettingRepository : IRepository<Setting, int>
{
    #region Cache Management

    /// <summary>
    /// بارگذاری اولیه تمام تنظیمات در Cache (فراخوانی در Startup)
    /// </summary>
    Task WarmupCacheAsync(CancellationToken cancellationToken = default);

    #endregion

    #region Query Methods

    /// <summary>
    /// دریافت همه تنظیمات یک گروه خاص (پیش‌فرض: General)
    /// </summary>
    Task<List<Setting>> GetByGroupAsync(string group = "General", CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت همه تنظیمات یک گروه خاص به صورت آبجکت strongly-typed (پیش‌فرض: General)
    /// </summary>
    Task<T> GetByGroupAsync<T>(string group = "General", CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت یک تنظیم خاص با گروه و کلید (پیش‌فرض: General)
    /// </summary>
    Task<Setting?> GetByKeyAsync(string key, string group = "General", CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت مقدار با تبدیل نوع - در صورت عدم تبدیل Exception میدهد
    /// </summary>
    Task<T?> GetValueAsync<T>(string key, string group = "General", CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت مقدار با تبدیل نوع - در صورت عدم تبدیل مقدار default نوع را برمیگرداند
    /// </summary>
    Task<T?> GetValueOrDefaultAsync<T>(string key, string group = "General", CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت مقدار با تبدیل نوع - در صورت عدم تبدیل مقدار defaultValue را برمیگرداند
    /// </summary>
    Task<T?> GetValueOrDefaultAsync<T>(string key, T? defaultValue, string group = "General", CancellationToken cancellationToken = default);

    #endregion

    #region Command Methods

    /// <summary>
    /// به‌روزرسانی مقدار یک تنظیم (پیش‌فرض: General)
    /// </summary>
    Task<bool> UpdateValueAsync(string key, string newValue, string group = "General", CancellationToken cancellationToken = default);

    #endregion

    #region Existence Checks

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

    #endregion
}