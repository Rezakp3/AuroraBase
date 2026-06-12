using Application.Common.Interfaces.ExternalServices;
using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using System.ComponentModel;
using System.Security.Cryptography;
using Utils.CustomAttributes;

namespace Application.Features.AuthFeature.AuthManagement.Commands;

public class SendOtpCommand : IBaseRequest
{
    [RequiredFa, DisplayName("شماره همراه"), StringLengthFa(13)]
    public string PhoneNumber { get; set; }
}

internal class SendOtpHandler(IUnitOfWork uow, ISmsService smsService) : IBaseHandler<SendOtpCommand>
{
    public async Task<ApiResult> Handle(SendOtpCommand request, CancellationToken cancellationToken)
    {
        var user = await uow.Users.GetByPhoneNumberOrUsernameAsync(request.PhoneNumber, cancellationToken);

        if (user == null)
            return ApiResult.NotFound("کاربر");

        if (user.OtpExpireDate > DateTime.UtcNow && user.TryCount < 3 && user.OtpCode is not null)
            return ApiResult.Fail("داداش کد قبلی هنوز منقضی نشده");

        var otp = RandomNumberGenerator.GetInt32(10000, 99999).ToString();

        user.OtpExpireDate = DateTime.UtcNow.AddMinutes(2);
        user.OtpCode = otp;
        user.TryCount = 0;

        await smsService.SendOtp(request.PhoneNumber, otp);
        var res = await uow.SaveChangesAsync(cancellationToken);

        return res ? ApiResult.Success(otp) : ApiResult.Fail();
    }
}