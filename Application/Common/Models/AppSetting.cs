namespace Application.Common.Models;

public class AppSetting
{
    public string AllowedHosts { get; set; } = string.Empty;
    public int UserRoleId { get; set; }
    public int AdminRoleId { get; set; }
    public ConnectionStrings ConnectionStrings { get; set; } = new();
    public JwtSettings JwtSettings { get; set; } = new();
}

public class ConnectionStrings
{
    public string DefaultConnection { get; set; } = string.Empty;
}

public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; set; }
    public int RefreshTokenExpirationDays { get; set; }
}