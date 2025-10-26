using System.ComponentModel;

namespace Utils.Helpers;

// این کلاس باید static باشد
public static class ConvertExtensions
{
    /// <summary>
    /// تلاش می‌کند یک رشته را به نوع مشخص شده تبدیل کند.
    /// </summary>
    /// <param name="value">رشته حاوی مقدار تنظیمات.</param>
    /// <param name="defaultValue">مقدار پیش‌فرضی که در صورت ناموفق بودن تبدیل برگردانده می‌شود.</param>
    /// <typeparam name="T">نوعی که رشته باید به آن تبدیل شود.</typeparam>
    /// <returns>مقدار تبدیل شده یا مقدار پیش‌فرض.</returns>
    public static T ConvertTo<T>(this string? value, T defaultValue = default!) 
        where T : IConvertible
    {
        // ۱. بررسی مقدار Null یا خالی
        if (string.IsNullOrWhiteSpace(value))
        {
            return defaultValue;
        }

        try
        {
            // ۲. استفاده از TypeConverter برای انواع خاص (مانند Nullable)
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter != null && converter.CanConvertFrom(typeof(string)))
            {
                return (T)converter.ConvertFromString(value)!;
            }

            // ۳. استفاده از Convert.ChangeType برای انواع پایه (int, bool, etc.)
            // اگر T یک نوع Nullable نیست، از Convert.ChangeType استفاده می‌کنیم.
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch (Exception)
        {
            // در صورت بروز هر گونه خطا در تبدیل (مثلاً رشته "abc" به int)
            return defaultValue;
        }
    }

    /// <summary>
    /// تبدیل رشته به نوع بولی (bool) با مدیریت مقادیر رایج True/False.
    /// </summary>
    public static bool ConvertToBool(this string? value, bool defaultValue = false)
    {
        if (bool.TryParse(value, out bool result))
        {
            return result;
        }
        // تبدیل مقادیر "1" و "0"
        if (value == "1") return true;
        if (value == "0") return false;

        return defaultValue;
    }
}
