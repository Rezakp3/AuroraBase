using Api.Attributes;
using Api.Controllers;
using Application.Features.AuthFeature.Commands; // ایمپورت Commandها
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Auth;

// مسیردهی: /Auth/LoginWithPassword
public class AuthController(IMediator mediator) : BaseController(mediator)
{
    /// <summary>
    /// ثبت نام کاربر جدید با رمز عبور
    /// </summary>
    [HttpPost]
    [AllowAnonymous] // ثبت نام نیازی به احراز هویت ندارد
    public Task<IActionResult> RegisterWithPassword([FromBody] RegisterWithPasswordCommand command) => Sender(command);

    /// <summary>
    /// ورود کاربر با نام کاربری/ایمیل و رمز عبور
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    public Task<IActionResult> LoginWithPassword([FromBody] LoginWithPasswordCommand command) => Sender(command);

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

    /// <summary>
    /// تست دسترسی پویا (فقط برای تست اولیه)
    /// این اکشن نیاز به مجوز Auth.Test دارد.
    /// </summary>
    [HttpGet]
    [AutoPermission]
    public IActionResult TestPermission() => Ok(new { Message = "شما دسترسی Auth.Test را دارید." });
}