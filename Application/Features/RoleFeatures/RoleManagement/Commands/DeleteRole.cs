using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Utils.CustomAttributes;

namespace Application.Features.RoleFeatures.RoleManagement.Commands;

public class DeleteRoleCommand : IBaseRequest
{
    [RequiredFa, DisplayName("آیدی")]
    public int Id { get; set; }
}

internal class DeleteRoleHandler(IUnitOfWork uow) : IBaseHandler<DeleteRoleCommand>
{
    public async Task<ApiResult> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await uow.Roles.GetByIdAsync(request.Id, cancellationToken);

        if (role is null)
            return ApiResult.NotFound("نقش");

        uow.Roles.Delete(role);

        var res = await uow.SaveChangesAsync(cancellationToken);

        return res.ToApiResult();
    }
}