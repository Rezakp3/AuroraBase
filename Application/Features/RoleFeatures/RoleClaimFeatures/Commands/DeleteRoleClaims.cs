using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Utils.CustomAttributes;

namespace Application.Features.RoleFeatures.RoleClaimFeatures.Commands;

public class DeleteRoleClaimsCommand : IBaseRequest
{
    [RequiredFa,DisplayName("آیدی")]
    public IEnumerable<int> Ids { get; set; }
}

internal class DeleteRoleClaimsHandler(IUnitOfWork uow) : IBaseHandler<DeleteRoleClaimsCommand>
{
    public async Task<ApiResult> Handle(DeleteRoleClaimsCommand request, CancellationToken cancellationToken)
    {
        uow.Roles.DeleteRoleClaims(request.Ids);
        
        var res = await uow.SaveChangesAsync(cancellationToken);

        return res.ToApiResult();
    }
}