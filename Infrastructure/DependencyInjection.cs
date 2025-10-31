using Application.Common.Interfaces.Generals;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services.Caching;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Seeders;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext با Interceptors
        services.AddDbContext<Persistence.MyContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(Persistence.MyContext).Assembly.FullName));
            
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
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<ISettingRepository, SettingRepository>();

        // ✅ ثبت Distributed Memory Cache
        services.AddDistributedMemoryCache();

        // ✅ ثبت Memory Cache برای قابلیت‌های پیشرفته
        services.AddMemoryCache();

        // ✅ ثبت سرویس مدیریت کش
        services.AddSingleton<ICacheService, CacheService>();
        return services;
    }

    // ✅ متد کمکی برای Seed کردن داده‌ها
    public static async Task SeedDatabaseAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MyContext>();
        
        await context.Database.MigrateAsync();
        await DefaultDataSeeder.SeedAsync(context);

        // ✅ Warmup Settings Cache
        var settingRepo = scope.ServiceProvider.GetRequiredService<ISettingRepository>();
        await settingRepo.WarmupCacheAsync();
    }
}
