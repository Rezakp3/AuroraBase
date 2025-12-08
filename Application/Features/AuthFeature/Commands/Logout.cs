using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Application.Features.Auth.Models;
using Aurora.Jwt.Helpers;
using Aurora.Jwt.Services.Jwt;
using Aurora.Jwt.Services.Token;
using MediatR;
using Microsoft.AspNetCore.Http;
using Utils.CustomAttributes;

namespace Application.Features.AuthFeature.Commands;

/// <summary>
/// مدل ورودی برای خروج کاربر
/// </summary>
public class LogoutCommand : IRequest<ApiResult>
{
    [RequiredFa(ErrorMessage = "Refresh Token")]
    public string RefreshToken { get; set; } = null!;
}

internal class LogoutCommandHandler(
    IUnitOfWork uow,
    IJwtService jwtService,
    ITokenManager tokenManager,
    IHttpContextAccessor accessor)
    : IRequestHandler<LogoutCommand, ApiResult>
{
    public async Task<ApiResult> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var accessToken = JwtTokenHelper.GetAccessToken(accessor);
        if (string.IsNullOrEmpty(accessToken))
        {
            return ApiResult<TokenVm>.Fail(message: "Access Token در هدر یافت نشد.", code: 401);
        }
        // 1. استخراج JTI از Access Token
        var principal = jwtService.GetPrincipalFromToken(accessToken);
        if (principal == null)
        {
            return ApiResult<TokenVm>.Fail(message: "توکن دسترسی نامعتبر است.", code: 401);
        }

        var jtiClaim = principal?.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti);
        var oldJti = jtiClaim?.Value;

        // 2. ابطال Access Token (بلک‌لیست)
        if (oldJti != null)
        {
            var expirationTimeClaim = principal?.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Exp);
            if (expirationTimeClaim != null && long.TryParse(expirationTimeClaim.Value, out var expUnix))
            {
                var expTime = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
                var remainingTime = expTime - DateTime.UtcNow;

                if (remainingTime > TimeSpan.Zero)
                {
                    await tokenManager.RevokeTokenAsync(oldJti, remainingTime, cancellationToken);
                }
            }
        }

        // 3. پیدا کردن و ابطال نشست (Session)
        var session = await uow.Sessions.GetByTokenAsync(request.RefreshToken, cancellationToken);

        if (session == null)
        {
            // اگر نشست پیدا نشد، ممکن است قبلاً منقضی یا باطل شده باشد. عملیات موفق است.
            return ApiResult.Success();
        }

        uow.Sessions.Delete(session);
        await uow.SaveChangesAsync(cancellationToken);

        return ApiResult.Success();
    }
}