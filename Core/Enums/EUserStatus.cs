using System.ComponentModel;

namespace Core.Enums;

public enum EUserStatus
{
    [Description("فعال")]
    Active = 1,

    [Description("غیرفعال")]
    InActive = 0,
}
