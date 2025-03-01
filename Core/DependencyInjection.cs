using Core.ViewModel.Base;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {

        #region AddResources
        //read resource and add it to static model ResourceConfig
        ResourceManager rmFa = new("Core.Resources.Resource-fa", Assembly.GetExecutingAssembly());
        ResourceConfig.ResourcesFa = rmFa.GetResourceSet(CultureInfo.CurrentCulture, true, true);
        #endregion


        return services;
    }
}
