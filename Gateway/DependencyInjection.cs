using Core.ViewModel.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway;

public static class DependencyInjection
{

    public static IServiceCollection AddGateway(this IServiceCollection services, AppSetting appSetting)
    {

        return services;
    }
}
