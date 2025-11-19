using Aurora.Cache.Services;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;

namespace Aurora.Cache;

public static class DependencyInjection
{
    public static IServiceCollection AddCache(this IServiceCollection services)
    {// 1. اضافه کردن HybridCache
        services.AddHybridCache(options =>
        {
            // تنظیمات دیفالت (مثل سایز کش محلی و ...)
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(30),
                LocalCacheExpiration = TimeSpan.FromMinutes(5)
            };
        });

        // 2. اتصال به Redis (برای L2)
        //services.AddStackExchangeRedisCache(options =>
        //{
        //    options.Configuration = builder.Configuration.GetConnectionString("Redis");
        //});

        // 3. تزریق سرویس خودتان
        services.AddSingleton<ICacheService, CacheService>();
        return services;
    }
}
