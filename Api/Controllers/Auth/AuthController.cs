using Api.Attributes;
using Application.Features.AuthFeature.AuthManagement.Commands;
using Application.Features.AuthFeature.AuthManagement.Queries;
using Application.Features.AuthFeature.SessionManagement.Commands;
using Application.Features.AuthFeature.SessionManagement.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Auth;

public class AuthController(IMediator mediator) : BaseController(mediator)
{
    #region login & register & logout
    [HttpGet]
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
    public async Task<IActionResult> Logout([FromBody] LogoutCommand command)
        => await Sender(command);

    #endregion

    #region Session

    [HttpGet]
    [AutoPermission]
    public async Task<IActionResult> SearchSession([FromQuery] SessionSearchQuery command)
        => await Sender(command);

    [HttpPost]
    [AutoPermission]
    public async Task<IActionResult> RevokeSession([FromBody] RevokeSessionCommand command)
        => await Sender(command);

    #endregion
}