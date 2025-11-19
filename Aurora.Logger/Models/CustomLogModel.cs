// رابط پایه برای اعمال محدودیت جنریک (Constraint) در متدها
namespace Aurora.Logger.Models;

// مدل پیش‌فرض که کاربر می‌تواند در صورت تمایل از آن استفاده کند.
public record CustomLogModel(
    string Message,
    object CustomData = null);