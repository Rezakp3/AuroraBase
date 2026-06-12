using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Application.Features.AuthFeature.AuthManagement.Models;
using Aurora.Jwt.Helpers;
using Aurora.Jwt.Models;
using Aurora.Jwt.Services.Jwt;
using Aurora.Jwt.Services.Token;
using Microsoft.AspNetCore.Http;
using Utils.CustomAttributes;

namespace Application.Features.AuthFeature.AuthManagement.Commands;

/// <summary>
/// مدل ورودی برای خروج کاربر
/// </summary>
public class LogoutCommand : IBaseRequest
{
    [RequiredFa(ErrorMessage = "Refresh Token")]
    public string RefreshToken { get; set; } = null!;
}

internal class LogoutCommandHandler(
    IUnitOfWork uow,
    ITokenManager tokenManager,
    IHttpContextAccessor accessor)
    : IBaseHandler<LogoutCommand>
{
    public async Task<ApiResult> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var jwtToken = accessor.GetToken<JwtVm>();

        if (jwtToken.Jti == null)
        {
            return ApiResult<TokenVm>.Fail(message: "اطلاعات توکن ناقص است.", code: 401);
        }

        var oldJti = jwtToken.Jti;

        // 2. ابطال Access Token (بلک‌لیست)
        if (oldJti != null)
        {
            var remainingTime = jwtToken.ExpirationDate - DateTime.UtcNow;

            if (remainingTime > TimeSpan.Zero)
            {
                await tokenManager.RevokeTokenAsync(oldJti, remainingTime, cancellationToken);
            }
        }

        // 3. پیدا کردن و ابطال نشست (Session)
        var session = await uow.Sessions.GetByTokenAsync(request.RefreshToken, cancellationToken);

        // اگر نشست پیدا نشد، ممکن است قبلاً منقضی یا باطل شده باشد. عملیات موفق است.
        if (session == null)
            return ApiResult.Success();

        uow.Sessions.Delete(session);
        await uow.SaveChangesAsync(cancellationToken);

        return ApiResult.Success();
    }
}