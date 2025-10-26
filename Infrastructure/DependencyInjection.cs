using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Persistence.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Application.Common.Interfaces.Generals;

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

        // Data Seeding (اختیاری - در Startup)
        // services.AddScoped<DefaultDataSeeder>();

        return services;
    }

    // ✅ متد کمکی برای Seed کردن داده‌ها
    public static async Task SeedDatabaseAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MyContext>();
        
        await context.Database.MigrateAsync();
        await DefaultDataSeeder.SeedAsync(context);
    }
}
