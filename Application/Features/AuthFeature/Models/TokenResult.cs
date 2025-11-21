using Application.Features.MenuFeature.Models;

namespace Application.Features.Auth.Models;

/// <summary>
/// نتیجه عملیات‌های احراز هویت (ورود، رفرش توکن)
/// </summary>
public class TokenVm
{
    /// <summary>
    /// توکن دسترسی (JWT)
    /// </summary>
    public string AccessToken { get; set; } = null!;

    /// <summary>
    /// توکن رفرش (برای تمدید نشست)
    /// </summary>
    public string RefreshToken { get; set; } = null!;

    /// <summary>
    /// زمان انقضای Access Token (به ثانیه)
    /// </summary>
    public DateTime ExpireTime { get; set; }

    public List<string> Roles { get; set; } = [];
    public List<MenuVm> Menus { get; set; } = [];
}
