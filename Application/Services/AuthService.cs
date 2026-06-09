using Application.Common.Interfaces.Generals;
using Application.Common.Interfaces.Services;
using Application.Features.Auth.Models;
using Application.Features.MenuFeature.Models;
using Application.Features.RoleFeatures.RoleManagement.Models;
using Application.Features.ServiceFeatures.Models;
using Aurora.Jwt.Models;
using Aurora.Jwt.Services.Jwt;
using Core.Entities.Auth;

namespace Application.Services;

public class AuthService(
    IUnitOfWork uow,
    IJwtService jwtService,
    IDeviceService deviceService) : IAuthService
{

    public async Task<TokenVm> GenerateAuthTokensAndSessionAsync(User user, CancellationToken ct)
    {
        // 1. دریافت اطلاعات دستگاه
        var clientInfo = deviceService.GetClientInfo();
        var jwt = await uow.Settings.GetByGroupAsync<JwtSettings>("JwtSettings", ct);
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
        IEnumerable<RoleDto> userRoles = await uow.Users.GetUserRoles(user.Id, ct);
        var roleNames = userRoles.Select(r => r.Name).ToList();
        var userRoleIds = userRoles.Select(r => r.Id);
        List<MenuDto> userMenus = await uow.Menus.GetMenusByRoleIds(userRoleIds, ct);
        List<ServiceDto> userServices = await uow.Services.GetServicesByRoleIds(userRoleIds, ct);
        

        // 5. تولید Access Token
        var payload = new TokenPayload
        {
            UserId = user.Id,
            Username = user.UserName,
            PhoneNumber = user.PhoneNumber,
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
            Menus = userMenus,
            Services = userServices,
        };
    }

    public async Task<TokenVm> RotateTokensAndSessionAsync(User user, Session oldSession, CancellationToken ct)
    {
        var jwt = await uow.Settings.GetByGroupAsync<JwtSettings>("JwtSettings", ct);
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
        IEnumerable<RoleDto> userRoles = await uow.Users.GetUserRoles(user.Id, ct);
        var roleNames = userRoles.Select(r => r.Name).ToList();
        var userRoleIds = userRoles.Select(r => r.Id);
        List<MenuDto> userMenus = await uow.Menus.GetMenusByRoleIds(userRoleIds, ct);
        List<ServiceDto> userServices = await uow.Services.GetServicesByRoleIds(userRoleIds, ct);

        // 5. تولید Access Token جدید
        var payload = new TokenPayload
        {
            UserId = user.Id,
            Username = user.UserName,
            PhoneNumber = user.PhoneNumber,
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
            Menus = userMenus,
            Services = userServices
        };
    }


}