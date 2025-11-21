using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Aurora.Jwt;
using Aurora.Jwt.Helpers;
using Aurora.Jwt.Services.Token;
using Microsoft.AspNetCore.Http;

namespace Aurora.Jwt.Middlewares;

public class JwtBlocklistMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ITokenManager tokenManager)
    {
        // 1. اگر کاربر احراز هویت نشده، یعنی توکنی پارس نشده، پس رد شو.
        if (context.User.Identity?.IsAuthenticated is not true)
        {
            await next(context);
            return;
        }

        // 2. استخراج JTI از Claims
        var jtiClaim = context.User.FindFirst(JwtRegisteredClaimNames.Jti)
                       ?? context.User.FindFirst(ClaimTypes.SerialNumber); // گاهی اوقات ممکنه اینجا مپ بشه

        if (jtiClaim != null)
        {
            // 3. چک کردن در کش
            var isRevoked = await tokenManager.IsTokenRevokedAsync(jtiClaim.Value);

            if (isRevoked)
            {
                // 4. توکن در بلک‌لیست است -> قطع درخواست
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Token has been revoked.");
                return;
            }
        }

        // 5. همه چیز مرتب است -> ادامه بده
        await next(context);
    }
}