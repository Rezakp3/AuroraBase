using Application.Common.Interfaces.ExternalServices;
using Application.Common.Interfaces.Generals;
using Application.Common.Interfaces.Repositories;
using Infrastructure.ExternalService.SmsService;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Repositories.Base;
using Infrastructure.Persistence.Repositories.Cached;
using Infrastructure.Persistence.Seeders;
using Infrastructure.Security.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Utils.Helpers;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var aa = configuration.GetConnectionString("DefaultConnection");
        // DbContext با Interceptors
        services.AddDbContext<MyContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(MyContext).Assembly.FullName))
                .EnableSensitiveDataLogging() // اختیاری: برای دیدن مقادیر پارامترها
                .LogTo(message => Debug.WriteLine(message), LogLevel.Information);

            // ✅ اضافه کردن Interceptor
            options.AddInterceptors(new AuditableEntityInterceptor());
        });

        // Unit of Work & Repository Pattern
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

        // ✅ Repository های مشخص
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IMenuRepository, MenuRepository>();
        services.AddScoped<IServiceRepository, ServiceRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();

        services.AddScoped<ISettingRepository, SettingRepository>();
        services.Decorate<ISettingRepository, CachedSettingRepository>();

        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.Decorate<IUserRoleRepository, CachedUserRoleRepository>();

        services.AddScoped<IRoleServiceRepository, RoleServiceRepository>();
        services.Decorate<IRoleServiceRepository, CachedRoleServiceRepository>();
        services.AddScoped<IAuthorizationHandler, DynamicPermissionHandler>();

        services.AddTransient<ISmsService, SmsIrService>();
        return services;
    }

    //  متد کمکی برای Seed کردن داده‌ها
    public static async Task SeedDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MyContext>();

        await context.Database.MigrateAsync();
        await DefaultDataSeeder.SeedAsync(context);

        // ✅ Warmup Settings Cache
        var settingRepo = scope.ServiceProvider.GetRequiredService<ISettingRepository>();
    }
}
