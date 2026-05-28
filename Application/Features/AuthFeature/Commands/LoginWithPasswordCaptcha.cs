using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Application.Features.Auth.Models;
using Aurora.Captcha.Services;
using Mapster;
using MediatR;

namespace Application.Features.AuthFeature.Commands;

public class LoginWithPasswordCaptchaCommand
    : LoginWithPasswordCommand, IBaseRequest<TokenVm>
{
    public string CaptchaId { get; set; }
    public string Captcha { get; set; }
}

internal class LoginWithPasswordCaptchaCommandHandler(ISender sender, ICaptchaService captchaService)
    : IBaseHandler<LoginWithPasswordCaptchaCommand, TokenVm>
{
    public async Task<ApiResult<TokenVm>> Handle(LoginWithPasswordCaptchaCommand request, CancellationToken cancellationToken)
    {
        var res = new ApiResult<TokenVm>();
        var captchaRes = await captchaService.ValidateAsync(request.CaptchaId, request.Captcha, false, cancellationToken);

        if (captchaRes is null)
            return res.Fail();

        if (!captchaRes.IsValid)
            return res.Fail(captchaRes.ErrorMessage);

        var loginCommand = request.Adapt<LoginWithPasswordCommand>();
        return await sender.Send(loginCommand, cancellationToken);
    }
}
