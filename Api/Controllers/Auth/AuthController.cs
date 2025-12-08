using Api.Attributes;
using Application.Features.AuthFeature;
using Application.Features.AuthFeature.Commands;
using Application.Features.AuthFeature.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Auth;

// مسیردهی: /Auth/LoginWithPassword
public class AuthController(IMediator mediator) : BaseController(mediator)
{
    #region login & register & logout

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetCaptcha()
    {
        var command = new GetCaptchaQuery();
        return await Sender(command);
    }

    /// <summary>
    /// ثبت نام کاربر جدید با رمز عبور
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    public Task<IActionResult> RegisterWithPassword([FromBody] RegisterWithPasswordCommand command) => Sender(command);

    /// <summary>
    /// ثبت نام کاربر جدید با رمز عبور
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    public Task<IActionResult> RegisterWithPasswordCaptcha([FromBody] RegisterWithPasswordCaptchaCommand command) => Sender(command);

    [HttpPost]
    [AllowAnonymous]
    public Task<IActionResult> LoginWithPassword([FromBody] LoginWithPasswordCommand command) => Sender(command);

    /// <summary>
    /// ورود کاربر با نام کاربری/ایمیل و رمز عبور
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    public Task<IActionResult> LoginWithPasswordCaptcha([FromBody] LoginWithPasswordCaptchaCommand command) => Sender(command);

    /// <summary>
    /// رفرش کردن Access Token با استفاده از Refresh Token
    /// Access Token منقضی شده از هدر Authorization خوانده می‌شود.
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    public Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command) => Sender(command);

    /// <summary>
    /// خروج از سیستم (ابطال نشست فعلی)
    /// Access Token فعال از هدر Authorization خوانده می‌شود.
    /// </summary>
    [HttpPost]
    [AutoPermission]
    public Task<IActionResult> Logout([FromBody] LogoutCommand command) => Sender(command);

    #endregion

    #region email verification

    [HttpPost]
    [AutoPermission]
    public async Task<IActionResult> SendEmailVerification()
        => await Sender(new SendEmailVerificationCommand());

    [HttpGet]
    [AutoPermission]
    public async Task<IActionResult> VeifyEmail([FromQuery] VerifyEmailCommand request)
        => await Sender(request);

    #endregion

    #region Change password

    [HttpPut]
    [AutoPermission]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand request)
        => await Sender(request);

    #endregion

    #region reset password

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> SendResetPasswordMail([FromBody] SendResetPasswordEmailCommand request)
        => await Sender(request);

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand request)
        => await Sender(request);

    #endregion
}