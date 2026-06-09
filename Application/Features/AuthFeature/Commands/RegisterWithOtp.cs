using Application.Common.Interfaces.ExternalServices;
using Application.Common.Interfaces.Generals;
using Application.Common.Interfaces.Services;
using Application.Common.Models;
using Application.Features.Auth.Models;
using Core.Entities.Auth;
using Core.Enums;
using System.ComponentModel;
using System.Security.Cryptography;
using Utils.CustomAttributes;

namespace Application.Features.AuthFeature.Commands;

public class RegisterWithOtpCommand : IBaseRequest
{
    [RequiredFa, DisplayName("شماره همراه"), StringLengthFa(13)]
    public string PhoneNumber { get; set; }

    [RequiredFa, DisplayName("نام"), StringLengthFa(100)]
    public string FName { get; set; }

    [DisplayName("نام خانوادگی"), StringLengthFa(100)]
    public string LName { get; set; }
}

internal class RegisterWithOtpHandler(
    IUnitOfWork uow,
    ISmsService smsService) : IBaseHandler<RegisterWithOtpCommand>
{
    public async Task<ApiResult> Handle(RegisterWithOtpCommand request, CancellationToken cancellationToken)
    {
        var exist = await uow.Users.PhoneNumberExistForAdd(request.PhoneNumber, cancellationToken);

        if (exist)
            return ApiResult.Fail("شماره همراه تکراری است");

        // 2. شروع تراکنش
        await uow.BeginTransactionAsync(cancellationToken);

        try
        {
            var otp = RandomNumberGenerator.GetInt32(10000, 99999);
            // 3. ایجاد موجودیت User و PasswordLogin
            var newUser = new User
            {
                Status = EUserStatus.Active,
                LastLoginDate = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
                PhoneNumber = request.PhoneNumber,
                LastUpdateDate = DateTime.UtcNow,
                OtpCode = otp.ToString(),
                TryCount = 0,
                OtpExpireDate = DateTime.UtcNow.AddMinutes(2),
                FName = request.FName,
                LName = request.LName
            };

            await uow.Users.AddAsync(newUser, cancellationToken);
            await uow.SaveChangesAsync(cancellationToken);

            var userRoleId = await uow.Settings.GetValueAsync<int>("UserRoleId", "Security", cancellationToken);

            // 4. انتساب نقش پیش‌فرض
            await uow.UserRoles.AssignRoleToUserAsync(newUser.Id, userRoleId, cancellationToken);


            await smsService.SendOtp(request.PhoneNumber, otp.ToString());

            // 6. پایان تراکنش
            await uow.CommitTransactionAsync(cancellationToken);

            return ApiResult.Success(otp.ToString());
        }
        catch (Exception)
        {
            await uow.RollbackTransactionAsync(cancellationToken);
            return ApiResult.Fail(code: 500);
        }
    }
}