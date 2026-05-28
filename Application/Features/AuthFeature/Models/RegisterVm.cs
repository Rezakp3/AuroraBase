using System.ComponentModel.DataAnnotations;
using Utils.CustomAttributes;

namespace Application.Features.AuthFeature.Models;

public class RegisterVm
{
    [RequiredFa(ErrorMessage = "نام کاربری")]
    [StringLengthFa(maximumLength: 20)]
    public string Username { get; set; } = null!;

    [RequiredFa(ErrorMessage = "رمز عبور")]
    [StringLengthFa(maximumLength: 50)]
    public string Password { get; set; } = null!;
}
