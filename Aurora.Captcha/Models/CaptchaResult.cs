namespace Aurora.Captcha.Models;

/// <summary>
/// نتیجه تولید کپچا
/// </summary>
public class CaptchaResult
{
    /// <summary>
    /// شناسه یکتای کپچا
    /// </summary>
    public string CaptchaId { get; set; } = null!;

    /// <summary>
    /// تصویر کپچا به صورت Base64
    /// </summary>
    public string ImageBase64 { get; set; } = null!;

    /// <summary>
    /// زمان انقضا (به ثانیه)
    /// </summary>
    public int ExpiresInSeconds { get; set; }
}