using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Utils.CustomAttributes;

namespace Application.Features.RoleFeatures.RoleClaimFeatures.Commands;

public class DeleteRoleClaimCommand : IBaseRequest
{
    [RequiredFa, DisplayName("آیدی")]
    public int Id { get; set; }
}

internal class DeleteRoleClaimHandler(IUnitOfWork uow) : IBaseHandler<DeleteRoleClaimCommand>
{
    public async Task<ApiResult> Handle(DeleteRoleClaimCommand request, CancellationToken cancellationToken)
    {
        var claim = await uow.Roles.GetClaimByIdAsync(request.Id, cancellationToken);

        if (claim is null)
            return ApiResult.NotFound("کلیم");

        uow.Roles.DeleteClaim(claim);
        
        var res = await uow.SaveChangesAsync(cancellationToken);

        return res.ToApiResult();
    }
}