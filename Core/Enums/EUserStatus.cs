using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Core.Enums;

public enum EUserStatus
{
    [Description("پیش فعال")]
    PreActive = 0,

    [Description("فعال")]
    Active = 1,

    [Description("غیرفعال")]
    InActive = 2,

    [Description("مسدود")]
    Blocked = 3,
}
