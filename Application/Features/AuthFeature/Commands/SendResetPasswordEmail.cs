using Application.Common.Interfaces.ExternalServices;
using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using EmailSender.Sevices;
using EmailSender.ViewModel;
using MediatR;
using System.ComponentModel.DataAnnotations;
using Utils.CustomAttributes;

namespace Application.Features.AuthFeature.Commands;

public class SendResetPasswordEmailCommand : IRequest<ApiResult>
{
    [RequiredFa(ErrorMessage = "ایمیل")]
    [StringLengthFa(maximumLength: 30)]
    [EmailAddress(ErrorMessage = "فرمت ایمیل نامعتبر است")]
    public string Email { get; set; } = null!;
}

internal class SendResetPasswordEmailCommandHandler(IUnitOfWork uow, IEmailService emailService) : IRequestHandler<SendResetPasswordEmailCommand, ApiResult>
{
    public async Task<ApiResult> Handle(SendResetPasswordEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await uow.PasswordLogin.GetByEmail(request.Email, cancellationToken);

        if (user is null)
            return ApiResult.NotFound("کاربر");

        var resetToken = Guid.NewGuid().ToString("N");

        user.ResetPasswordToken = resetToken;
        user.ResetPasswordTokenExpireDate = DateTime.UtcNow.AddHours(1);
        uow.PasswordLogin.Update(user);
        var saveResult = await uow.SaveChangesAsync(cancellationToken);

        if (!saveResult)
            return ApiResult.Fail("خطا در ایجاد توکن بازیابی رمز عبور");

        var emailBody = $"برای بازیابی رمز عبور خود، لطفاً روی لینک زیر کلیک کنید:\n" +
                        $"https://baseFront.com/reset-password?token={resetToken}\n" +
                        $"این لینک تا یک ساعت معتبر است.";

        var mail = new SendEmailVm()
        {
            TextBody = emailBody,
            Reciever = request.Email,
            Subject = "بازیابی رمز عبور"
        };
        var emailResult = await emailService.SendAsync(mail, cancellationToken);
        
        if (!emailResult)
            return ApiResult.Fail("خطا در ارسال ایمیل بازیابی رمز عبور");
       
        return ApiResult.Success("ایمیل بازیابی رمز عبور با موفقیت ارسال شد");
    }
}