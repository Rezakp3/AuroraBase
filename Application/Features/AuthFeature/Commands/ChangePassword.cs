using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Aurora.Jwt.Helpers;
using Aurora.Jwt.Services.Token;
using Microsoft.AspNetCore.Http;
using Utils.CustomAttributes;
using Utils.Helpers;
using IBaseRequest = Application.Common.Interfaces.Generals.IBaseRequest;

namespace Application.Features.AuthFeature.Commands;

public class ChangePasswordCommand : IBaseRequest
{
    [RequiredFa(ErrorMessage = "رمز عبور")]
    [StringLengthFa(maximumLength: 50)]
    public string CurrentPassword { get; set; }

    [RequiredFa(ErrorMessage = "رمز عبور")]
    [StringLengthFa(maximumLength: 50)]
    public string NewPassword { get; set; }

    [RequiredFa(ErrorMessage = "رمز عبور")]
    [StringLengthFa(maximumLength: 50)]
    public string NewPasswordRepeat { get; set; }
}

internal class ChangePasswordCommandHandler(IUnitOfWork uow,ITokenManager tokenManager,IHttpContextAccessor accessor)
    : IBaseHandler<ChangePasswordCommand>
{
    public async Task<ApiResult> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = accessor.GetUserId<long>();
        var pl = await uow.Users.GetByIdAsync(userId, cancellationToken);

        if (pl is null)
            return ApiResult.NotFound("کاربر");

        if (request.NewPassword != request.NewPasswordRepeat)
            return ApiResult.Fail("رمز عبور جدید با تکرار آن مطابقت ندارد");

        if (!PasswordHasher.VerifyPassword(request.CurrentPassword, pl.PasswordHash))
            return ApiResult.NotFound("رمز عبور فعلی");

        pl.PasswordHash = PasswordHasher.HashPassword(request.NewPassword);
        uow.Users.Update(pl);

        var sessions = await uow.Sessions.GetActiveTokensByUserIdAsync(userId, cancellationToken);

        uow.Sessions.DeleteRange(sessions);
        var deleteRes = await uow.SaveChangesAsync(cancellationToken);

        foreach (var session in sessions)
        {
            var expireTime = session.ExpireDate - DateTime.UtcNow;
            await tokenManager.RevokeTokenAsync(session.Jti, expireTime, cancellationToken);
        }

        return deleteRes ? ApiResult.Success("رمز عبور با موفقیت تغییر یافت") : ApiResult.Fail("خطا در تغییر رمز عبور");
    }
}
