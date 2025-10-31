namespace Application.Common.Models.Captcha;

/// <summary>
/// داده‌های کپچا برای ذخیره در کش
/// </summary>
public class CaptchaData
{
    /// <summary>
    /// کد اصلی کپچا
    /// </summary>
    public string Code { get; set; } = null!;

    /// <summary>
    /// زمان ایجاد
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// تعداد تلاش‌های باقی‌مانده
    /// </summary>
    public int RemainingAttempts { get; set; } = 3;

    /// <summary>
    /// آیا استفاده شده؟
    /// </summary>
    public bool IsUsed { get; set; }
}