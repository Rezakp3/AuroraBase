using System.ComponentModel;

namespace Application.Common.Models;

public class AppSetting
{
    public string AllowedHosts { get; set; } = string.Empty;
    public int UserRoleId { get; set; }
    public int AdminRoleId { get; set; }
    public ConnectionStrings ConnectionStrings { get; set; } = new();
}


public class ConnectionStrings
{
    public string DefaultConnection { get; set; } = string.Empty;
}

