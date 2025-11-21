using Application.Common.Interfaces.Generals;
using Application.Common.Interfaces.Services;
using Application.Features.Auth.Models;
using Application.Features.MenuFeature.Models;
using Aurora.Jwt.Models;
using Aurora.Jwt.Services.Jwt;
using Core.Entities.Auth;

namespace Application.Services;

public class AuthService(
    IUnitOfWork uow,
    IJwtService jwtService,
    IDeviceService deviceService,
    JwtSettings jwt) : IAuthService
{

    public async Task<TokenVm> GenerateAuthTokensAndSessionAsync(User user, CancellationToken ct)
    {
        // 1. دریافت اطلاعات دستگاه
        var clientInfo = deviceService.GetClientInfo();

        // 2. تولید JTI و Refresh Token
        var jti = Guid.NewGuid().ToString("N");
        var refreshToken = JwtService.GenerateRefreshToken();
        var accessTokenExpiration = TimeSpan.FromMinutes(jwt.AccessTokenExpirationMinutes);
        var refreshTokenExpiration = TimeSpan.FromDays(jwt.RefreshTokenExpirationDays);

        // 3. ایجاد نشست (Session)
        var session = new Session
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Jti = jti,
            RefreshToken = refreshToken,
            ExpireDate = DateTime.UtcNow.Add(refreshTokenExpiration),
            DeviceName = $"{clientInfo.OperatingSystem} - {clientInfo.Browser}",
            IsRevoked = false
        };
        await uow.Sessions.AddAsync(session, ct);

        // 4. دریافت نقش‌ها و منوها
        var (roleNames, menuVms) = await GetUserAuthDataAsync(user.Id, ct);

        // 5. تولید Access Token
        var payload = new TokenPayload
        {
            UserId = user.Id,
            Username = user.PasswordLogin.UserName,
            Email = user.PasswordLogin.Email,
            Jti = jti,
            Roles = roleNames,
        };
        var accessToken = jwtService.GenerateAccessToken(payload);

        // 6. ذخیره تغییرات (در Command اصلی SaveChangesAsync صدا زده می‌شود)
        // توجه: در این متد، SaveChangesAsync را صدا نمی‌زنیم تا Command اصلی بتواند آن را در تراکنش مدیریت کند.

        return new TokenVm
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpireTime = DateTime.UtcNow.Add(accessTokenExpiration),
            Roles = roleNames,
            Menus = menuVms
        };
    }

    public async Task<TokenVm> RotateTokensAndSessionAsync(User user, Session oldSession, CancellationToken ct)
    {
        // 1. تولید JTI و Refresh Token جدید
        var newJti = Guid.NewGuid().ToString("N");
        var newRefreshToken = JwtService.GenerateRefreshToken();
        var accessTokenExpiration = TimeSpan.FromMinutes(jwt.AccessTokenExpirationMinutes);
        var refreshTokenExpiration = TimeSpan.FromDays(jwt.RefreshTokenExpirationDays);

        // 2. به‌روزرسانی نشست قدیمی (Revoke کردن آن)
        oldSession.Revoke();
        uow.Sessions.Update(oldSession);

        // 3. ایجاد نشست جدید
        var newSession = new Session
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Jti = newJti,
            RefreshToken = newRefreshToken,
            ExpireDate = DateTime.UtcNow.Add(refreshTokenExpiration),
            DeviceName = oldSession.DeviceName, // اطلاعات دستگاه را حفظ می‌کنیم
            IsRevoked = false
        };
        await uow.Sessions.AddAsync(newSession, ct);

        // 4. دریافت نقش‌ها و منوها
        var (roleNames, menuVms) = await GetUserAuthDataAsync(user.Id, ct);

        // 5. تولید Access Token جدید
        var payload = new TokenPayload
        {
            UserId = user.Id,
            Username = user.PasswordLogin.UserName,
            Email = user.PasswordLogin.Email,
            Jti = newJti,
            Roles = roleNames,
        };
        var newAccessToken = jwtService.GenerateAccessToken(payload);

        return new TokenVm
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpireTime = DateTime.UtcNow.Add(accessTokenExpiration),
            Roles = roleNames,
            Menus = menuVms
        };
    }

    /// <summary>
    /// متد داخلی برای دریافت نقش‌ها و منوهای مجاز کاربر
    /// </summary>
    private async Task<(List<string> roleNames, List<MenuVm> menuVms)> GetUserAuthDataAsync(long userId, CancellationToken ct)
    {
        var roleIds = await uow.UserRoles.GetUserRoleIdsAsync(userId, ct);
        var roles = await uow.Roles.GetWhereAsync(r => roleIds.Contains(r.Id), ct);
        var roleNames = roles.Select(r => r.Name).ToList();

        // دریافت منوهای مجاز
        var allMenus = new HashSet<Menu>();
        foreach (var roleId in roleIds)
        {
            var roleMenus = await uow.Menus.GetByRoleId(roleId, ct);
            foreach (var menu in roleMenus)
            {
                allMenus.Add(menu);
            }
        }
        var menuVms = BuildMenuTree(allMenus.ToList());

        return (roleNames, menuVms);
    }

    /// <summary>
    /// تبدیل لیست مسطح منوها به ساختار درختی
    /// </summary>
    private static List<MenuVm> BuildMenuTree(List<Menu> flatMenus)
    {
        var menuDict = flatMenus.ToDictionary(m => m.Id, m => new MenuVm
        {
            Id = m.Id,
            Title = m.Title,
            Route = m.Route,
            ParentId = m.ParentId
        });

        var rootMenus = new List<MenuVm>();

        foreach (var menuVm in menuDict.Values)
        {
            if (menuVm.ParentId.HasValue && menuDict.TryGetValue(menuVm.ParentId.Value, out var parent))
            {
                parent.SubMenus.Add(menuVm);
            }
            else
            {
                rootMenus.Add(menuVm);
            }
        }

        return rootMenus;
    }
}