namespace Utils.Helpers;

/// <summary>
/// کلاس کمکی برای ساخت کلیدهای کش به صورت استاندارد
/// </summary>
public static class CacheKeyBuilder
{
    // Prefixes برای جلوگیری از تداخل کلیدها
    public const string CaptchaPrefix = "captcha";
    public const string UserPrefix = "user";
    public const string SettingsPrefix = "settings";
    public const string SessionPrefix = "session";
    public const string TokenPrefix = "token";

    /// <summary>
    /// ساخت کلید کش برای کپچا
    /// </summary>
    /// <param name="captchaId">شناسه کپچا</param>
    /// <returns>کلید کش</returns>
    public static string ForCaptcha(string captchaId)
        => $"{CaptchaPrefix}:{captchaId}";

    /// <summary>
    /// ساخت کلید کش برای اطلاعات تلاش‌های کپچا
    /// </summary>
    /// <param name="captchaId">شناسه کپچا</param>
    /// <returns>کلید کش</returns>
    public static string ForCaptchaAttempts(string captchaId)
        => $"{CaptchaPrefix}:attempts:{captchaId}";

    /// <summary>
    /// ساخت کلید کش برای کاربر
    /// </summary>
    /// <param name="userId">شناسه کاربر</param>
    /// <returns>کلید کش</returns>
    public static string ForUser(long userId)
        => $"{UserPrefix}:{userId}";

    /// <summary>
    /// ساخت کلید کش برای تنظیمات
    /// </summary>
    /// <param name="group">گروه تنظیمات</param>
    /// <param name="key">کلید تنظیم</param>
    /// <returns>کلید کش</returns>
    public static string ForSetting(string group, string key)
        => $"{SettingsPrefix}:{group}:{key}";

    /// <summary>
    /// ساخت کلید کش برای Session
    /// </summary>
    /// <param name="sessionId">شناسه Session</param>
    /// <returns>کلید کش</returns>
    public static string ForSession(string sessionId)
        => $"{SessionPrefix}:{sessionId}";

    /// <summary>
    /// ساخت کلید کش برای Token
    /// </summary>
    /// <param name="tokenId">شناسه Token</param>
    /// <returns>کلید کش</returns>
    public static string ForToken(string tokenId)
        => $"{TokenPrefix}:{tokenId}";

    /// <summary>
    /// ساخت کلید کش سفارشی
    /// </summary>
    /// <param name="prefix">پیشوند</param>
    /// <param name="parts">اجزای کلید</param>
    /// <returns>کلید کش</returns>
    public static string Custom(string prefix, params string[] parts)
        => $"{prefix}:{string.Join(":", parts)}";
}