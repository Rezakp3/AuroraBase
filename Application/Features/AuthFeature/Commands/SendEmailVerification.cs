using Application.Common.Interfaces.ExternalServices;
using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Aurora.Jwt.Helpers;
using EmailSender.Sevices;
using EmailSender.ViewModel;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.AuthFeature.Commands;

public class SendEmailVerificationCommand : IRequest<ApiResult>;

internal class SendEmailVerificationCommandHandler(IUnitOfWork uow, IEmailService emailService, IHttpContextAccessor accessor) : IRequestHandler<SendEmailVerificationCommand, ApiResult>
{
    public async Task<ApiResult> Handle(SendEmailVerificationCommand request, CancellationToken cancellationToken)
    {
        var userId = accessor.GetUserId<long>();
        var pl = await uow.PasswordLogin.GetByUserId(userId, cancellationToken);

        if (pl is not null && pl.EmailIsVerified)
            return ApiResult.Fail("ایمیل قبلا تایید شده");

        if ((pl.EmailVerificationCode is null && pl.VerifyCodeExpireDate is null)
            || pl.VerifyCodeExpireDate < DateTime.UtcNow)
        {

            var verifyCode = Guid.NewGuid().ToString("N");

            pl.EmailVerificationCode = verifyCode;
            pl.VerifyCodeExpireDate = DateTime.UtcNow.AddHours(1);

            uow.PasswordLogin.Update(pl);
            var res = await uow.SaveChangesAsync(cancellationToken);

            if (!res)
                return ApiResult.Fail();

        }

        var mail = new SendEmailVm
        {
            Reciever = pl.Email,
            Subject = pl.EmailVerificationCode,
            TextBody = pl.EmailVerificationCode
        };

        var sendEmailRes = await emailService.SendAsync(mail, cancellationToken);

        if (!sendEmailRes)
            return ApiResult.Fail();

        return ApiResult.Success();
    }
}