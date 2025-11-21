// Application/Features/Auth/Commands/Register/RegisterCommandHandler.cs

using Application.Common.Interfaces.Generals;
using Application.Common.Interfaces.Services;
using Application.Common.Models;
using Application.Features.Auth.Models;
using MediatR;
using Microsoft.Extensions.Options;
using Core.Entities.Auth;
using Core.Enums;
using Utils.Helpers;

namespace Application.Features.AuthFeature.Commands;

public class RegisterWithPasswordCommand : RegisterCommand, IRequest<ApiResult<TokenVm>>;
public class RegisterWithPasswordCommandHandler(
    IUnitOfWork uow,
    IAuthService authService, // تزریق سرویس جدید
    AppSetting appSettings)
    : IRequestHandler<RegisterWithPasswordCommand, ApiResult<TokenVm>>
{

    public async Task<ApiResult<TokenVm>> Handle(RegisterWithPasswordCommand request, CancellationToken cancellationToken)
    {
        // 1. بررسی تکراری بودن
        var exists = await uow.Users.UserNameOrEmailExistForAdd(request.Email, request.Username, cancellationToken);
        if (exists)
        {
            return ApiResult<TokenVm>.Fail(code: 1001);
        }

        // 2. هش کردن رمز عبور
        var hashedPassword = PasswordHasher.HashPassword(request.Password);

        // 3. شروع تراکنش
        await uow.BeginTransactionAsync(cancellationToken);

        try
        {
            // 4. ایجاد موجودیت User و PasswordLogin
            var newUser = new User
            {
                Status = EUserStatus.Active,
                LastLoginDate = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
                PasswordLogin = new PasswordLogin
                {
                    UserName = request.Username,
                    Email = request.Email,
                    PasswordHash = hashedPassword,
                    LastUpdateDate = DateTime.UtcNow,
                }
            };
            await uow.Users.AddAsync(newUser, cancellationToken);
            await uow.SaveChangesAsync(cancellationToken);

            // 5. انتساب نقش پیش‌فرض
            await uow.UserRoles.AssignRoleToUserAsync(newUser.Id, appSettings.UserRoleId, cancellationToken);

            // 6. تولید توکن‌ها و نشست (استفاده از سرویس مشترک)
            var tokenResult = await authService.GenerateAuthTokensAndSessionAsync(newUser, cancellationToken);

            // 7. پایان تراکنش
            await uow.CommitTransactionAsync(cancellationToken);

            return ApiResult<TokenVm>.Success(tokenResult);
        }
        catch (Exception)
        {
            await uow.RollbackTransactionAsync(cancellationToken);
            return ApiResult<TokenVm>.Fail(code: 500);
        }
    }
}