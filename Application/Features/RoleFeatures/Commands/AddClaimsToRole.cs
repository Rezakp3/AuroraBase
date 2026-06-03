using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Application.Features.RoleFeatures.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Utils.CustomAttributes;

namespace Application.Features.RoleFeatures.Commands;

public class AddClaimsToRoleCommand : IBaseRequest
{
    [RequiredFa, DisplayName("نقش")]
    public int RoleId { get; set; }
    public IEnumerable<RoleClaimDto> Claims { get; set; }
}

internal class AddClaimsToRoleHandler(IUnitOfWork uow) : IBaseHandler<AddClaimsToRoleCommand>
{
    public async Task<ApiResult> Handle(AddClaimsToRoleCommand request, CancellationToken cancellationToken)
    {
        uow.Roles.AddClaims(request.RoleId, request.Claims,cancellationToken);

        var res = await uow.SaveChangesAsync(cancellationToken);

        return res.ToApiResult();
    }
}