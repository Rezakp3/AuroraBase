using Application.Common.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        // MediatR Registration
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
        });

        // Resource Configuration
        InitializeResources();

        return services;
    }

    private static void InitializeResources()
    {
        try
        {
            // فرض می‌کنیم فایل Resource در پوشه Application/Resources قرار دارد
            var resourceManager = new ResourceManager(
                "Application.Resources.Resource-fa", 
                Assembly.GetExecutingAssembly());
            
            ResourceConfig.ResourcesFa = resourceManager.GetResourceSet(
                CultureInfo.CurrentCulture, 
                createIfNotExists: true, 
                tryParents: true);
        }
        catch (Exception)
        {
            // اگر Resource file وجود نداشت، null می‌ماند
            ResourceConfig.ResourcesFa = null;
        }
    }
}
