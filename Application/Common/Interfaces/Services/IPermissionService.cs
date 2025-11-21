namespace Application.Common.Interfaces.Services;

public interface IPermissionService
{
    /// <summary>
    /// بررسی می‌کند آیا کاربر دسترسی خاصی را دارد یا خیر.
    /// این متد باید بسیار سریع باشد چون در هر ریکوئست صدا زده می‌شود.
    /// </summary>
    Task<bool> HasPermissionAsync(long userId, string permissionIdentifier, CancellationToken ct = default);

    /// <summary>
    /// لیست تمام دسترسی‌های یک کاربر را برمی‌گرداند (مثلاً برای ارسال به فرانت‌اند).
    /// </summary>
    Task<HashSet<string>> GetUserPermissionsAsync(long userId, CancellationToken ct = default);
}

