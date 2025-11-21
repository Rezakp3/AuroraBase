using Aurora.Captcha.Models;

namespace Aurora.Captcha.Services;

/// <summary>
/// سرویس مدیریت کپچا
/// </summary>
public interface ICaptchaService
{
    /// <summary>
    /// تولید کپچای جدید
    /// </summary>
    /// <param name="length">طول کد (پیش‌فرض: 5)</param>
    /// <param name="expirationMinutes">زمان انقضا به دقیقه (پیش‌فرض: 5)</param>
    /// <param name="maxAttempts">حداکثر تعداد تلاش (پیش‌فرض: 3)</param>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>نتیجه تولید کپچا</returns>
    Task<CaptchaResult> GenerateAsync(
        int length = 5, 
        int expirationMinutes = 2, 
        int maxAttempts = 3,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// اعتبارسنجی کپچا
    /// </summary>
    /// <param name="captchaId">شناسه کپچا</param>
    /// <param name="userInput">ورودی کاربر</param>
    /// <param name="caseSensitive">حساس به حروف بزرگ/کوچک (پیش‌فرض: false)</param>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>نتیجه اعتبارسنجی</returns>
    Task<CaptchaValidationResult> ValidateAsync(
        string captchaId, 
        string userInput, 
        bool caseSensitive = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// حذف کپچا (بعد از استفاده موفق)
    /// </summary>
    /// <param name="captchaId">شناسه کپچا</param>
    /// <param name="cancellationToken">توکن لغو</param>
    Task RemoveAsync(string captchaId, CancellationToken cancellationToken = default);

    /// <summary>
    /// تولید مجدد کپچا با همان ID
    /// </summary>
    /// <param name="captchaId">شناسه کپچای قبلی</param>
    /// <param name="length">طول کد</param>
    /// <param name="expirationMinutes">زمان انقضا</param>
    /// <param name="maxAttempts">حداکثر تلاش</param>
    /// <param name="cancellationToken">توکن لغو</param>
    /// <returns>نتیجه تولید کپچا</returns>
    Task<CaptchaResult> RegenerateAsync(
        string captchaId, 
        int length = 5, 
        int expirationMinutes = 5,
        int maxAttempts = 3,
        CancellationToken cancellationToken = default);
}