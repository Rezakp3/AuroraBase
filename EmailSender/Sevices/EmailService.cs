using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;
using EmailSender.ViewModel;

namespace EmailSender.Sevices;

public class EmailService(EmailSettings setting) : IEmailService
{
    public async Task<bool> SendAsync(SendEmailVm emailVm, CancellationToken cancellationToken = default)
    {
        // ۱. ساخت پیام با MimeKit (استاندارد و قوی)
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("نام فرستنده", setting.Sender));
        message.To.Add(new MailboxAddress("نام گیرنده", emailVm.Reciever));
        message.Subject = emailVm.Subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = emailVm.HtmlBody,
            TextBody = emailVm.TextBody
        };
        message.Body = bodyBuilder.ToMessageBody();

        // ۲. استفاده از SmtpClient از فضای نام MailKit
        using var client = new SmtpClient();
        // اتصال به سرور SMTP گوگل با پورت و SSL/TLS
        await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls, cancellationToken);

        // احراز هویت با ایمیل و رمز عبور برنامه
        await client.AuthenticateAsync(setting.Sender, setting.Password, cancellationToken);

        // ارسال پیام
        var res = await client.SendAsync(message);

        // قطع اتصال
        await client.DisconnectAsync(true, cancellationToken);

        return true;
    }
}
