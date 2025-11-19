namespace Aurora.Jwt.Services.Token;

public interface ITokenManager
{
    Task RevokeTokenAsync(string jti, TimeSpan expiration, CancellationToken ct = default);
    Task<bool> IsTokenRevokedAsync(string jti, CancellationToken ct = default);
}
