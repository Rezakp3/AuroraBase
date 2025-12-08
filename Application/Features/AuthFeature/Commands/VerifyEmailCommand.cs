using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Aurora.Jwt.Helpers;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.AuthFeature.Commands;

public class VerifyEmailCommand : IRequest<ApiResult>
{
    public string VerificationCode { get; set; }
}
internal class VerifyEmailCommandHandler(IUnitOfWork uow, IHttpContextAccessor accessor) : IRequestHandler<VerifyEmailCommand, ApiResult>
{
    public async Task<ApiResult> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var res = new ApiResult();
        var userId = accessor.GetUserId<int>();
        var pl = await uow.PasswordLogin.GetByUserId(userId, cancellationToken);

        if (pl.EmailVerificationCode != request.VerificationCode)
            return res.Fail("لینک نامعتبر");

        if (pl.VerifyCodeExpireDate < DateTime.UtcNow)
            return res.Fail("لینک منقضی شده است");

        pl.EmailIsVerified = true;
        pl.EmailVerificationCode = null;
        pl.VerifyCodeExpireDate = null;
        uow.PasswordLogin.Update(pl);

        var updateRes = await uow.SaveChangesAsync(cancellationToken);

        return updateRes ? res.Success() : res.Fail();
    }
}