using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Utils.CustomAttributes;

namespace Application.Features.Auth.Models;

public class RegisterCommand
{
    [RequiredFa(ErrorMessage = "نام کاربری")]
    [StringLengthFa(maximumLength: 20)]
    public string Username { get; set; } = null!;

    [RequiredFa(ErrorMessage = "ایمیل")]
    [StringLengthFa(maximumLength: 30)]
    [EmailAddress(ErrorMessage = "فرمت ایمیل نامعتبر است")]
    public string Email { get; set; } = null!;

    [RequiredFa(ErrorMessage = "رمز عبور")]
    [StringLengthFa(maximumLength: 50)]
    public string Password { get; set; } = null!;
}
