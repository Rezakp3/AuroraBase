using Application.Features.MenuFeature.Models;

namespace Application.Features.Auth.Models;

/// <summary>
/// نتیجه عملیات‌های احراز هویت (ورود، رفرش توکن)
/// </summary>
public class TokenVm
{
    public string AccessToken { get; set; } = null!;

    public string RefreshToken { get; set; } = null!;

    public DateTime ExpireTime { get; set; }

    public List<string> Roles { get; set; } = [];
    public List<MenuVm> Menus { get; set; } = [];
}
