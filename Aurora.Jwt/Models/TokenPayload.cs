namespace Aurora.Jwt.Models;

/// <summary>
/// مدل اطلاعات مورد نیاز برای تولید توکن
/// </summary>
public class TokenPayload
{
    /// <summary>
    /// شناسه کاربر (الزامی)
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// نام کاربری
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// ایمیل کاربر
    /// </summary>
    public string Email { get; set; }

    public string Jti { get; set; }

    /// <summary>
    /// لیست نقش‌های کاربر
    /// </summary>
    public List<string> Roles { get; set; }

    /// <summary>
    /// Claim های سفارشی (مثل Permissions, DeviceId و غیره)
    /// </summary>
    public Dictionary<string, string> CustomClaims { get; set; }
}
