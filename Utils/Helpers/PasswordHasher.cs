namespace Utils.Helpers;


/// <summary>
/// کلاس کمکی برای هش کردن و اعتبارسنجی رمز عبور با استفاده از BCrypt
/// </summary>
public static class PasswordHasher
{
    /// <summary>
    /// هش کردن رمز عبور با استفاده از الگوریتم BCrypt
    /// </summary>
    /// <param name="password">رمز عبور خام</param>
    /// <returns>رشته هش شده</returns>
    public static string HashPassword(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password, workFactor: 10);

    /// <summary>
    /// اعتبارسنجی رمز عبور خام با هش ذخیره شده
    /// </summary>
    /// <param name="password">رمز عبور خام ورودی</param>
    /// <param name="hashedPassword">هش ذخیره شده در دیتابیس</param>
    /// <returns>true اگر رمز عبور معتبر باشد</returns>
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashedPassword))
            return false;

        // BCrypt.Verify به صورت خودکار Salt و Work Factor را از هش استخراج می‌کند.
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}