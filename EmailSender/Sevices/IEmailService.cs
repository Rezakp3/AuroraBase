using EmailSender.ViewModel;

namespace EmailSender.Sevices;

public interface IEmailService
{
    Task<bool> SendAsync(SendEmailVm emailVm, CancellationToken cancellationToken = default);
}