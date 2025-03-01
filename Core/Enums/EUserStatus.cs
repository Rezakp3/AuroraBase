using System.ComponentModel.DataAnnotations;

namespace Core.Enums;

public enum EUserStatus
{
    [Display(Name = "فعال")]
    Active = 1,

    [Display(Name = "غیرفعال")]
    InActive = 0,

    [Display(Name = "مسدود")]
    Blocked = 2,
}
