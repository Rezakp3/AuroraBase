using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Utils.CustomAttributes;

namespace Application.Features.UsersFeature.UserManagement.Commands;

public class ChangeUserStatusCommand : IBaseRequest
{
    [RequiredFa, DisplayName("کاربر")]
    public long UserId { get; set; }
    [RequiredFa, DisplayName("وضعیت")]
    public EUserStatus Status { get; set; }
}

internal class ChangeUserStatusHandler(IUnitOfWork uow)
    : IBaseHandler<ChangeUserStatusCommand>
{
    public async Task<ApiResult> Handle(ChangeUserStatusCommand request, CancellationToken cancellationToken)
    {
        var user = await uow.Users.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
            return ApiResult.NotFound("کاربر");

        user.Status = request.Status;
        var res = await uow.SaveChangesAsync(cancellationToken);

        return res.ToApiResult();
    }
}