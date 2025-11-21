using Application.Common.Interfaces.Generals;
using Application.Common.Interfaces.Services;
using Application.Common.Models;
using Application.Features.Auth.Models;
using Core.Enums;
using MediatR;
using Utils.CustomAttributes;
using Utils.Helpers;

namespace Application.Features.AuthFeature.Commands;

public class LoginWithPasswordCommand : IRequest<ApiResult<TokenVm>>
{
    [RequiredFa(ErrorMessage = "نام کاربری یا ایمیل")]
    public string UsernameOrEmail { get; set; } = null!;

    [RequiredFa(ErrorMessage = "رمز عبور")]
    public string Password { get; set; } = null!;
}
public class LoginWithPasswordCommandHandler(
    IUnitOfWork uow,
    IAuthService authService) // تزریق سرویس مشترک
    : IRequestHandler<LoginWithPasswordCommand, ApiResult<TokenVm>>
{
    public async Task<ApiResult<TokenVm>> Handle(LoginWithPasswordCommand request, CancellationToken cancellationToken)
    {
        // 1. پیدا کردن کاربر و اعتبارسنجی رمز عبور
        var user = await uow.Users.GetByEmailOrUsernameAsync(request.UsernameOrEmail, cancellationToken);

        if (user?.PasswordLogin == null ||
            !PasswordHasher.VerifyPassword(request.Password, user.PasswordLogin.PasswordHash))
        {
            return ApiResult<TokenVm>.Fail(code: 401);
        }

        // 2. بررسی وضعیت کاربر
        if (user.Status != EUserStatus.Active)
        {
            return ApiResult<TokenVm>.Fail(message: "حساب کاربری فعال نیست یا مسدود شده است.", code: 401);
        }

        // 3. تولید توکن‌ها و نشست (استفاده از سرویس مشترک)
        var tokenResult = await authService.GenerateAuthTokensAndSessionAsync(user, cancellationToken);

        // 4. به‌روزرسانی تاریخ آخرین ورود
        user.LastLoginDate = DateTime.UtcNow;
        uow.Users.Update(user);
        await uow.SaveChangesAsync(cancellationToken);

        return ApiResult<TokenVm>.Success(tokenResult);
    }
}