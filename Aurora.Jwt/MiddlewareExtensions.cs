using Aurora.Jwt.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace Aurora.Jwt;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseJwtBlocklist(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<JwtBlocklistMiddleware>();
    }
}