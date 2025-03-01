using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Enums;

public enum EPlatform
{
    [Display(Name = "اندروید")]
    Android = 0,

    [Display(Name = "آی او اس")]
    Ios = 1,

    [Display(Name = "وب")]
    Web = 2,
}
