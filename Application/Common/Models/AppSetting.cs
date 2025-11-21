using Aurora.Jwt.Models;
using Aurora.Logger.Models;

namespace Application.Common.Models;

public class AppSetting
{
    public string AllowedHosts { get; set; } = string.Empty;
    public int UserRoleId { get; set; }
    public int AdminRoleId { get; set; }
    public ConnectionStrings ConnectionStrings { get; set; } = new();
    public JwtSettings JwtSettings { get; set; }
    public AuroraLogSettings AuroraLog { get; set; }
}


public class ConnectionStrings
{
    public string DefaultConnection { get; set; } = string.Empty;
}

