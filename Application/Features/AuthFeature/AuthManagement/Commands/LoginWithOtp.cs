using Application.Common.Interfaces.Generals;
using Application.Common.Interfaces.Services;
using Application.Common.Models;
using Application.Features.AuthFeature.AuthManagement.Models;
using Application.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Utils.CustomAttributes;

namespace Application.Features.AuthFeature.AuthManagement.Commands;

public class LoginWithOtpCommand : IBaseRequest<TokenVm>
{
    [RequiredFa, DisplayName("کد یکبار مصرف"), StringLengthFa(13)]
    public string Otp { get; set; }

    [RequiredFa, DisplayName("شماره همراه"), StringLengthFa(13)]
    public string PhoneNumber { get; set; }
}

internal class LoginWithOtpHandler(IUnitOfWork uow, IAuthService authService) : IBaseHandler<LoginWithOtpCommand, TokenVm>
{
    public async Task<ApiResult<TokenVm>> Handle(LoginWithOtpCommand request, CancellationToken cancellationToken)
    {
        var user = await uow.Users.GetByPhoneNumberOrUsernameAsync(request.PhoneNumber, cancellationToken);

        if (user == null)
            return ApiResult<TokenVm>.NotFound("کاربر");

        if (user.OtpExpireDate < DateTime.UtcNow)
            return ApiResult<TokenVm>.Fail("مهلت استفاده از کد تایید به پایان رسیده است");

        if (user.TryCount >= 3 && user.OtpExpireDate > DateTime.UtcNow)
            return ApiResult<TokenVm>.Fail("تلاش بیش از حد مجاز");

        if (user.OtpCode != request.Otp)
        {
            user.TryCount += 1;
            uow.Users.Update(user);
            uow.SaveChanges();
            return ApiResult<TokenVm>.Fail("کد تایید اشتباه است");
        }

        user.TryCount = 0;
        user.OtpCode = null;
        user.OtpExpireDate = null;
        uow.Users.Update(user);

        // 5. تولید توکن‌ها و نشست (استفاده از سرویس مشترک)
        var tokenResult = await authService.GenerateAuthTokensAndSessionAsync(user, cancellationToken);
        await uow.SaveChangesAsync(cancellationToken);

        return ApiResult<TokenVm>.Success(tokenResult);
    }
}