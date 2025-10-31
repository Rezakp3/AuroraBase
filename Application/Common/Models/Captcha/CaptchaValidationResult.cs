namespace Application.Common.Models.Captcha;

/// <summary>
/// نتیجه اعتبارسنجی کپچا
/// </summary>
public class CaptchaValidationResult
{
    /// <summary>
    /// آیا کپچا معتبر است؟
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// پیام خطا (در صورت عدم اعتبار)
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// تعداد تلاش‌های باقی‌مانده
    /// </summary>
    public int? RemainingAttempts { get; set; }

    public static CaptchaValidationResult Success()
        => new() { IsValid = true };

    public static CaptchaValidationResult Failure(string errorMessage, int? remainingAttempts = null)
        => new() 
        { 
            IsValid = false, 
            ErrorMessage = errorMessage, 
            RemainingAttempts = remainingAttempts 
        };
}