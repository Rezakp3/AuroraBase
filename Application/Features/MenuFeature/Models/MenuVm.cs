// Application/Features/Menu/Models/MenuDto.cs

namespace Application.Features.MenuFeature.Models;

/// <summary>
/// مدل داده‌ای برای نمایش آیتم‌های منو در فرانت‌اند
/// </summary>
public class MenuVm
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Route { get; set; } = null!;
    public int? ParentId { get; set; }

    /// <summary>
    /// زیرمنوها
    /// </summary>
    public List<MenuVm> SubMenus { get; set; } = [];

    // اگر نیاز به آیکون یا ترتیب باشد، اینجا اضافه می‌شود
    // public string Icon { get; set; }
    // public int Order { get; set; }
}