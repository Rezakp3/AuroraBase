using Api.Attributes;
using Application.Features.AuthFeature.Commands;
using Application.Features.AuthFeature.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Auth;

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

    [HttpPost]
    public async Task<IActionResult> RegisterWithOtp([FromBody] RegisterWithOtpCommand command) => await Sender(command);

    [HttpPost]
    public async Task<IActionResult> SendOtp([FromBody] SendOtpCommand command) => await Sender(command);

    [HttpPost]
    public async Task<IActionResult> LoginWithOtp([FromBody] LoginWithOtpCommand command) => await Sender(command);

    /// <summary>
    /// رفرش کردن Access Token با استفاده از Refresh Token
    /// Access Token منقضی شده از هدر Authorization خوانده می‌شود.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command) => await Sender(command);

    /// <summary>
    /// خروج از سیستم (ابطال نشست فعلی)
    /// Access Token فعال از هدر Authorization خوانده می‌شود.
    /// </summary>
    [HttpPost]
    [AutoPermission(true)]
    public async Task<IActionResult> Logout([FromBody] LogoutCommand command) => await Sender(command);

    #endregion

}