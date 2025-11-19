using Aurora.Jwt.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Aurora.Jwt.Services.Jwt;

public interface IJwtService
{
    /// <summary>
    /// تولید Access Token با استفاده از اطلاعات کاربر و تنظیمات JWT
    /// </summary>
    /// <param name="payload">اطلاعات مورد نیاز برای قرار دادن در توکن</param>
    /// <param name="jwtSettings">تنظیمات JWT از appsettings</param>
    /// <returns>Access Token به صورت string</returns>
    public string GenerateAccessToken(TokenPayload payload);

    /// <summary>
    /// تولید Refresh Token به صورت تصادفی و امن
    /// </summary>
    /// <returns>Refresh Token به صورت string</returns>
    public static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    /// <summary>
    /// استخراج Claims از یک JWT Token
    /// </summary>
    /// <param name="token">JWT Token</param>
    /// <param name="jwtSettings">تنظیمات JWT</param>
    /// <returns>لیست Claims</returns>
    public ClaimsPrincipal? GetPrincipalFromToken(string token);
}
