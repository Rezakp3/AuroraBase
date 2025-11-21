using Aurora.Jwt;
using Aurora.Jwt.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Aurora.Jwt.Services.Jwt;

public class JwtService(JwtSettings options) : IJwtService
{
    /// <summary>
    /// تولید Access Token با استفاده از اطلاعات کاربر و تنظیمات JWT
    /// </summary>
    /// <param name="payload">اطلاعات مورد نیاز برای قرار دادن در توکن</param>
    /// <param name="options">تنظیمات JWT از appsettings</param>
    /// <returns>Access Token به صورت string</returns>
    public string GenerateAccessToken(TokenPayload payload)
    {
        var claims = BuildClaims(payload);
        var signingCredentials = GetSigningCredentials(options.SecretKey);

        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(options.AccessTokenExpirationMinutes),
            signingCredentials: signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// تولید Refresh Token به صورت تصادفی و امن
    /// </summary>
    /// <returns>Refresh Token به صورت string</returns>
    public static string GenerateRefreshToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    /// <summary>
    /// استخراج Claims از یک JWT Token
    /// </summary>
    /// <param name="token">JWT Token</param>
    /// <param name="options">تنظیمات JWT</param>
    /// <returns>لیست Claims</returns>
    public ClaimsPrincipal? GetPrincipalFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(options.SecretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false, // برای Refresh Token که منقضی شده
                ValidateIssuerSigningKey = true,
                ValidIssuer = options.Issuer,
                ValidAudience = options.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                return null;

            return principal;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// ساخت لیست Claims از Payload
    /// </summary>
    private static List<Claim> BuildClaims(TokenPayload payload)
    {
        var claims = new List<Claim>
        {
            new("UserId",payload.UserId.ToString()),
            new(JwtRegisteredClaimNames.Jti, payload.Jti),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };

        // اضافه کردن Username اگر وجود داشته باشد
        if (!string.IsNullOrWhiteSpace(payload.Username))
        {
            claims.Add(new Claim("Username", payload.Username));
        }

        // اضافه کردن Email اگر وجود داشته باشد
        if (!string.IsNullOrWhiteSpace(payload.Email))
        {
            claims.Add(new Claim("Email", payload.Email));
        }

        // اضافه کردن Roles
        if (payload.Roles != null && payload.Roles.Count != 0)
        {
            claims.AddRange(payload.Roles.Select(role => new Claim("Role", role)));
        }

        // اضافه کردن Custom Claims
        if (payload.CustomClaims != null && payload.CustomClaims.Count != 0)
        {
            claims.AddRange(payload.CustomClaims.Select(kvp => new Claim(kvp.Key, kvp.Value)));
        }

        return claims;
    }

    /// <summary>
    /// ساخت SigningCredentials
    /// </summary>
    private static SigningCredentials GetSigningCredentials(string secretKey)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }

    /// <summary>
    /// بررسی الگوریتم امضای توکن
    /// </summary>
    private static bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
    {
        return validatedToken is JwtSecurityToken jwtSecurityToken &&
               jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                   StringComparison.InvariantCultureIgnoreCase);
    }
}