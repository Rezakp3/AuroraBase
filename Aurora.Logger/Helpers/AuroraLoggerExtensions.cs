// رابط پایه برای اعمال محدودیت جنریک (Constraint) در متدها
using Aurora.Logger.Helpers;
using Aurora.Logger.Models;
using Aurora.Logger.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aurora.Logger.Helpers;
public static class AuroraLoggerExtensions
{
    extension(IServiceCollection services)
    {
        // متد ۱: دریافت تنظیمات از IConfiguration با ورودی نام سکشن
        public IServiceCollection AddAuroraLogger(
            IConfiguration configuration,
            string sectionName = "AuroraLogSettings")
        {
            services.Configure<AuroraLogOptions>(configuration.GetSection(sectionName));
            services.AddScoped<IAuroraLogger, AuroraLogger>();
            return services;
        }

        // متد ۲: دریافت OptionsBuilder برای پیکربندی از طریق کد
        public IServiceCollection AddAuroraLogger(
            Action<AuroraLogOptions> configureOptions)
        {
            services.Configure(configureOptions);
            services.AddScoped<IAuroraLogger, AuroraLogger>();
            return services;
        }
    }
}