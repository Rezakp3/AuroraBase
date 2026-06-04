using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Application.Features.RoleFeatures.RoleClaimFeatures.Models;
using Core.Entities.Auth;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Utils.CustomAttributes;

namespace Application.Features.RoleFeatures.RoleClaimFeatures.Commands;

public class UpdateRoleClaimCommand : RoleClaimDto, IBaseRequest
{
    [RequiredFa, DisplayName("آیدی")]
    public int Id { get; set; }
}

internal class UpdateRoleClaimHandler(IUnitOfWork uow) : IBaseHandler<UpdateRoleClaimCommand>
{
    public async Task<ApiResult> Handle(UpdateRoleClaimCommand request, CancellationToken cancellationToken)
    {
        var claim = await uow.Roles.GetClaimByIdAsync(request.Id, cancellationToken);
        if (claim is null)
            return ApiResult.NotFound("کلیم");
        claim.Type = request.Type;
        claim.Value = request.Value;
        var res = await uow.SaveChangesAsync(cancellationToken);
        return res.ToApiResult();
    }
}
