using Aurora.Jwt.Services.Jwt;
using Aurora.Jwt.Services.Token;
using Microsoft.Extensions.DependencyInjection;

namespace Aurora.Jwt;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddJwt()
        {
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<ITokenManager, TokenManager>();
            return services;
        }
    }
}
