namespace Aurora.Jwt.Models;

// مدلی برای نگهداری اطلاعات کاربر استخراج شده از توکن
public class JwtVm : TokenPayload
{
    // Issued At - زمان صدور توکن (معمولاً به صورت Unix Epoch Time)
    public long Iat { get; set; }

    // Not Before - توکن از چه زمانی معتبر است (Unix Epoch Time)
    public long Nbf { get; set; }

    // Expiration Time - زمان انقضای توکن (Unix Epoch Time)
    public long Exp { get; set; }

    // Issuer - صادرکننده توکن
    public string Iss { get; set; } = string.Empty;

    // Audience - گیرنده یا مصرف‌کننده توکن
    public string Aud { get; set; } = string.Empty;


    // --- متد کمکی برای تبدیل زمان‌های Unix به DateTime ---
    public DateTime ExpirationDate => DateTimeOffset.FromUnixTimeSeconds(Exp).UtcDateTime;
    public DateTime IssuedAtDate => DateTimeOffset.FromUnixTimeSeconds(Iat).UtcDateTime;
}
