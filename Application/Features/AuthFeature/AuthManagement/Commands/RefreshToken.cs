using Application.Common.Interfaces.Generals;
using Application.Common.Interfaces.Services;
using Application.Common.Models;
using Application.Features.AuthFeature.AuthManagement.Models;
using Aurora.Jwt.Services.Token;
using System.ComponentModel;
using Utils.CustomAttributes;

namespace Application.Features.AuthFeature.AuthManagement.Commands;

public class RefreshTokenCommand : IBaseRequest<TokenVm>
{
    [RequiredFa, DisplayName("توکن")]
    public string RefreshToken { get; set; } = null!;
}

internal class RefreshTokenCommandHandler(
    IUnitOfWork uow,
    ITokenManager tokenManager,
    IAuthService authService) // استفاده از سرویس مشترک
    : IBaseHandler<RefreshTokenCommand, TokenVm>
{
    public async Task<ApiResult<TokenVm>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {

        // 2. پیدا کردن نشست (Session)
        var session = await uow.Sessions.GetByTokenAsync(request.RefreshToken, cancellationToken);

        // 3. اعتبارسنجی نشست
        if (session == null || session.UserId != session.UserId || session.IsRevoked || session.IsExpired)
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
        var remainingTime = session.ExpireDate - DateTime.UtcNow;

        if (remainingTime < TimeSpan.Zero)
        {
            await tokenManager.RevokeTokenAsync(session.Jti, remainingTime, cancellationToken);
        }


        // 5. تولید توکن‌های جدید و چرخش نشست (استفاده از AuthService)
        var user = await uow.Users.GetByIdForAuthAsync(session.UserId, cancellationToken);
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