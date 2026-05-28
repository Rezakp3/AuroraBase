using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Aurora.Jwt.Services.Token;
using Utils.CustomAttributes;
using Utils.Helpers;

namespace Application.Features.AuthFeature.Commands;

public class ResetPasswordCommand : IBaseRequest
{
    public string ResetPasswordToken { get; set; }

    [RequiredFa(ErrorMessage = "رمز عبور")]
    [StringLengthFa(maximumLength: 50)]
    public string NewPassword { get; set; }

    [RequiredFa(ErrorMessage = "رمز عبور")]
    [StringLengthFa(maximumLength: 50)]
    public string NewPasswordRepeat { get; set; }
}

internal class ResetPasswordCommandHandler(IUnitOfWork uow,ITokenManager tokenManager) : IBaseHandler<ResetPasswordCommand>
{
    public async Task<ApiResult> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        if (request.NewPassword != request.NewPasswordRepeat)
            return ApiResult.Fail("رمزهای عبور مطابقت ندارند");

        var user = await uow.Users.GetByResetToken(request.ResetPasswordToken, cancellationToken);
        if (user is null || user.ResetPasswordTokenExpireDate < DateTime.UtcNow)
            return ApiResult.Fail("توکن بازیابی رمز عبور نامعتبر یا منقضی شده است");

        user.PasswordHash = PasswordHasher.HashPassword(request.NewPassword);
        user.ResetPasswordToken = null;
        user.ResetPasswordTokenExpireDate = null;
        uow.Users.Update(user);
        var saveResult = await uow.SaveChangesAsync(cancellationToken);

        if (!saveResult)
            return ApiResult.Fail("خطا در به‌روزرسانی رمز عبور");

        //اکسپایر و لاگ اوت کردن همه توکن های قدیمی
        var sessions = await uow.Sessions.GetActiveTokensByUserIdAsync(user.Id, cancellationToken);

        uow.Sessions.DeleteRange(sessions);
        await uow.SaveChangesAsync(cancellationToken);

        foreach (var session in sessions)
            await tokenManager.RevokeTokenAsync(session.Jti,session.ExpireDate - DateTime.UtcNow,cancellationToken);

        return ApiResult.Success("رمز عبور با موفقیت به‌روزرسانی شد");
    }
}