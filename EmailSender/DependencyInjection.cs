using EmailSender.Sevices;
using Microsoft.Extensions.DependencyInjection;

namespace EmailSender;

public static class DependencyInjection
{
    public static IServiceCollection AddEmailSender(this IServiceCollection services)
    {
        services.AddTransient<IEmailService, EmailService>();
        return services;
    }
}
