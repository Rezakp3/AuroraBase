using System.Text.Json;

namespace Utils.Helpers;

// این کلاس باید static باشد
public static class ConvertExtensions
{
    /// <summary>
    /// تبدیل string به نوع مورد نظر
    /// </summary>
    public static T ConvertTo<T>(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return default;

        var targetType = typeof(T);
        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        // برای انواع ساده
        if (underlyingType == typeof(string))
            return (T)(object)value;

        if (underlyingType == typeof(int))
            return (T)(object)int.Parse(value);

        if (underlyingType == typeof(long))
            return (T)(object)long.Parse(value);

        if (underlyingType == typeof(decimal))
            return (T)(object)decimal.Parse(value);

        if (underlyingType == typeof(double))
            return (T)(object)double.Parse(value);

        if (underlyingType == typeof(bool))
            return (T)(object)bool.Parse(value);

        if (underlyingType == typeof(DateTime))
            return (T)(object)DateTime.Parse(value);

        if (underlyingType == typeof(Guid))
            return (T)(object)Guid.Parse(value);

        // برای انواع پیچیده (JSON)
        return JsonSerializer.Deserialize<T>(value);
    }

    /// <summary>
    /// تبدیل رشته به نوع بولی (bool) با مدیریت مقادیر رایج True/False.
    /// </summary>
    public static bool ConvertToBool(this string value, bool defaultValue = false)
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
