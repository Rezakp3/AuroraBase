using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Aurora.Captcha.Models;
using Aurora.Captcha.Services;

namespace Application.Features.AuthFeature.Queries;

public class GetCaptchaQuery : IBaseRequest<CaptchaResult>;

internal class GetCaptchaQueryHandler(ICaptchaService captcha)
    : IBaseHandler<GetCaptchaQuery, CaptchaResult>
{
    public async Task<ApiResult<CaptchaResult>> Handle(GetCaptchaQuery request, CancellationToken cancellationToken)
    {
        var genereatedCaptcha = await captcha.GenerateAsync(5, 2, 3, cancellationToken);
        if (genereatedCaptcha is null)
            return ApiResult<CaptchaResult>.Fail();

        return ApiResult<CaptchaResult>.Success(genereatedCaptcha);
    }
}