using System.ComponentModel.DataAnnotations;

namespace Core.Enums;

public enum EUserSex
{
    [Display(Name = "نامشخص")]
    Unknown = 0,

    [Display(Name = "مرد")]
    Male = 1,

    [Display(Name = "زن")]
    Female = 2,
}
