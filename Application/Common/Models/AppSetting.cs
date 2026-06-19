using Aurora.Jwt.Models;
using Aurora.Logger.Models;
using Utils.Models;

namespace Application.Common.Models;

public class AppSetting
{
    public string AllowedHosts { get; set; } = string.Empty;
    public ConnectionStrings ConnectionStrings { get; set; } = new();
    public JwtSettings JwtSettings { get; set; }
    public AuroraLogSettings AuroraLog { get; set; }
    public CryptoSettings CryptoSettings { get; set; }
}


public class ConnectionStrings
{
    public string DefaultConnection { get; set; } = string.Empty;
}

