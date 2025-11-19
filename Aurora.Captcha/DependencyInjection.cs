using Aurora.Captcha.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Aurora.Captcha;

public static class DependencyInjection
{
    public static IServiceCollection AddCaptcha(this IServiceCollection services)
    {
        services.AddScoped<ICaptchaService, CaptchaService>();
        return services;
    }
}
