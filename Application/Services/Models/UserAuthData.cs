using Application.Features.MenuFeature.MenuManagement.Models;
using Application.Features.ServiceFeatures.Models;

namespace Application.Services.Models;

public class UserAuthData
{
    public IEnumerable<string> RoleNames { get; set; }
    public IEnumerable<MenuDto>  Menus { get; set; }
    public IEnumerable<ServiceDto> Services { get; set; }
}
