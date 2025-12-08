using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using Utils.CustomAttributes;
using Utils.Helpers;

namespace Application.Features.AuthFeature;

public class ResetPasswordCommand : IRequest<ApiResult>
{
    public string ResetPasswordToken { get; set; }

    [RequiredFa(ErrorMessage = "رمز عبور")]
    [StringLengthFa(maximumLength: 50)]
    public string NewPassword { get; set; }

    [RequiredFa(ErrorMessage = "رمز عبور")]
    [StringLengthFa(maximumLength: 50)]
    public string NewPasswordRepeat { get; set; }
}
internal class ResetPasswordCommandHandler(IUnitOfWork uow) : IRequestHandler<ResetPasswordCommand, ApiResult>
{
    public async Task<ApiResult> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        if (request.NewPassword != request.NewPasswordRepeat)
            return ApiResult.Fail("رمزهای عبور مطابقت ندارند");
        var pl = await uow.PasswordLogin.GetByResetToken(request.ResetPasswordToken, cancellationToken);
        if (pl is null || pl.ResetPasswordTokenExpireDate < DateTime.UtcNow)
            return ApiResult.Fail("توکن بازیابی رمز عبور نامعتبر یا منقضی شده است");
        // Here you would hash the password before saving it
        pl.PasswordHash = PasswordHasher.HashPassword(request.NewPassword);
        pl.ResetPasswordToken = null;
        pl.ResetPasswordTokenExpireDate = DateTime.MinValue;
        uow.PasswordLogin.Update(pl);
        var saveResult = await uow.SaveChangesAsync(cancellationToken);
        if (!saveResult)
            return ApiResult.Fail("خطا در به‌روزرسانی رمز عبور");
        return ApiResult.Success("رمز عبور با موفقیت به‌روزرسانی شد");
    }
}