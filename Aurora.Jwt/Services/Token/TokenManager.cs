using Aurora.Cache.Services;

namespace Aurora.Jwt.Services.Token;

public class TokenManager(ICacheService cache) : ITokenManager
{
    private const string Prefix = "blacklist:token:";

    public async Task RevokeTokenAsync(string jti, TimeSpan expiration, CancellationToken ct = default)
    {
        // ذخیره در کش با زمان انقضای باقیمانده توکن
        await cache.SetAsync($"{Prefix}{jti}", true, expiration, ct);
    }

    public async Task<bool> IsTokenRevokedAsync(string jti, CancellationToken ct = default)
    {
        return await cache.ExistsAsync($"{Prefix}{jti}", ct);
    }
}