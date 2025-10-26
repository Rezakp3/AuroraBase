namespace Application.Common.Interfaces.ExternalServices;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
}