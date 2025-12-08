using Application.Common.Models;
using Application.Features.Auth.Models;
using Aurora.Captcha.Services;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.AuthFeature.Commands;

public class RegisterWithPasswordCaptchaCommand : RegisterCommand, IRequest<ApiResult<TokenVm>>
{
    public string CaptchaId { get; set; }
    public string Captcha { get; set; }
}

internal class RegisterWithPasswordCaptchaCommandHandler(ISender sender, ICaptchaService captchaService) : IRequestHandler<RegisterWithPasswordCaptchaCommand, ApiResult<TokenVm>>
{
    public async Task<ApiResult<TokenVm>> Handle(RegisterWithPasswordCaptchaCommand request, CancellationToken cancellationToken)
    {
        var res = new ApiResult<TokenVm>(); 
        var captchaRes = await captchaService.ValidateAsync(request.CaptchaId, request.Captcha, false, cancellationToken);
       
        if(captchaRes is null)
            return res.Fail();
        
        if (!captchaRes.IsValid)
            return res.Fail(captchaRes.ErrorMessage);

        var registerCommand = request.Adapt<RegisterWithPasswordCommand>();
        return await sender.Send(registerCommand, cancellationToken);
    }
}