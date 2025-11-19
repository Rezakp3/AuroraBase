using Aurora.Logger.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Aurora.Logger;

public static class DependencyInjection
{
    public static IServiceCollection AddLogger(this IServiceCollection services)
    {
        services.AddScoped<IAuroraLogger, AuroraLogger>();
        return services;
    }
}
