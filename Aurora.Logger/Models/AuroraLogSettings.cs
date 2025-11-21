// رابط پایه برای اعمال محدودیت جنریک (Constraint) در متدها
namespace Aurora.Logger.Models;

// مدل تنظیمات برای DI
public class AuroraLogSettings
{
    public bool IsEnabled { get; set; } = true;
    public string DefaultLogCategory { get; set; } = "Aurora";
}