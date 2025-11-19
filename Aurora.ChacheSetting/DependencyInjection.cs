using Aurora.ChacheSetting.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Aurora.ChacheSetting;

public static class DependencyInjection
{
    public static IServiceCollection AddSetting(this IServiceCollection services)
    {
        services.AddSingleton<ISettingCacheProvider, SettingCacheProvider>();
        return services;
    }
}
