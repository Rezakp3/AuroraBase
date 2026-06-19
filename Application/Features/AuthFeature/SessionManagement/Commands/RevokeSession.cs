using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Aurora.Jwt.Services.Token;

namespace Application.Features.AuthFeature.SessionManagement.Commands;

public class RevokeSessionCommand : IBaseRequest
{
    public Guid Id { get; set; }
}

internal class RevokeSessionHandler(IUnitOfWork uow,ITokenManager tm)
    : IBaseHandler<RevokeSessionCommand>
{
    public async Task<ApiResult> Handle(RevokeSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await uow.Sessions.GetByIdAsync(request.Id,cancellationToken);

        if (session is null)
            return ApiResult.NotFound("نشست");

        if (session.IsExpired)
            return ApiResult.Success("این نشست منقضی شده است");

        await tm.RevokeTokenAsync(session.Jti, session.ExpireDate - DateTime.UtcNow, cancellationToken);
        uow.Sessions.Delete(session);
        await uow.SaveChangesAsync(cancellationToken);

        return ApiResult.Success();
    }
}