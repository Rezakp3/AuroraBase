using System;
using System.ComponentModel;
using System.Reflection;

namespace Utils.Helpers;

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        // گرفتن اطلاعات تایپ اینام
        Type type = value.GetType();

        // پیدا کردن نام دقیق فیلد انتخاب شده
        string name = Enum.GetName(type, value);
        if (name == null) return value.ToString();

        // گرفتن اتریبیوت دیسکریپشن از روی فیلد
        FieldInfo field = type.GetField(name);
        if (field == null) return value.ToString();

        DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();

        // اگر اتریبیوت وجود داشت، Description را برمی‌گرداند؛ در غیر این صورت خود نام اینام را دکمه می‌کند
        return attribute != null ? attribute.Description : value.ToString();
    }
}