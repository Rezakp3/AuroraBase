using Application.Common.Interfaces.Generals;
using Application.Common.Interfaces.Services;
using Application.Common.Models;
using Application.Features.Auth.Models;
using Aurora.Jwt.Helpers;
using Aurora.Jwt.Models;
using Aurora.Jwt.Services.Jwt;
using Aurora.Jwt.Services.Token;
using Core.Entities.Auth;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.Security.Claims;
using Utils.CustomAttributes;

namespace Application.Features.AuthFeature.Commands;

public class RefreshTokenCommand : IRequest<ApiResult<TokenVm>>
{
    [RequiredFa, DisplayName("توکن")]
    public string RefreshToken { get; set; } = null!;
}

internal class RefreshTokenCommandHandler(
    IUnitOfWork uow,
    ITokenManager tokenManager,
    IHttpContextAccessor accessor,
    IAuthService authService) // استفاده از سرویس مشترک
    : IRequestHandler<RefreshTokenCommand, ApiResult<TokenVm>>
{
    public async Task<ApiResult<TokenVm>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var jwtToken = accessor.GetToken<JwtVm>();

        if (jwtToken.Jti == null)
        {
            return ApiResult<TokenVm>.Fail(message: "اطلاعات توکن ناقص است.", code: 401);
        }

        var oldJti = jwtToken.Jti;

        // 2. پیدا کردن نشست (Session)
        var session = await uow.Sessions.GetByTokenAsync(request.RefreshToken, cancellationToken);

        // 3. اعتبارسنجی نشست
        if (session == null || session.UserId != jwtToken.UserId || session.IsRevoked || session.IsExpired)
        {
            if (session != null)
            {
                session.Revoke();
                uow.Sessions.Update(session);
                await uow.SaveChangesAsync(cancellationToken);
            }
            return ApiResult<TokenVm>.Fail(message: "نشست نامعتبر یا منقضی شده است. لطفاً مجدداً وارد شوید.", code: 401);
        }

        // 4. ابطال Access Token قدیمی (بلک‌لیست)
        var expirationTimeClaim = jwtToken.Exp;
        var expTime = DateTimeOffset.FromUnixTimeSeconds(expirationTimeClaim).UtcDateTime;
        var remainingTime = expTime - DateTime.UtcNow;

        if (remainingTime > TimeSpan.Zero)
        {
            await tokenManager.RevokeTokenAsync(oldJti, remainingTime, cancellationToken);
        }


        // 5. تولید توکن‌های جدید و چرخش نشست (استفاده از AuthService)
        var user = await uow.Users.GetByIdForAuthAsync(jwtToken.UserId, cancellationToken);
        if (user == null || user.Status != Core.Enums.EUserStatus.Active)
        {
            return ApiResult<TokenVm>.Fail(message: "کاربر یافت نشد یا غیرفعال است.", code: 401);
        }

        var tokenResult = await authService.RotateTokensAndSessionAsync(user, session, cancellationToken);

        // 6. ذخیره تغییرات (توسط RotateTokensAndSessionAsync در AuthService انجام نمی‌شود، پس اینجا انجام می‌دهیم)
        await uow.SaveChangesAsync(cancellationToken);

        return ApiResult<TokenVm>.Success(tokenResult);
    }
}