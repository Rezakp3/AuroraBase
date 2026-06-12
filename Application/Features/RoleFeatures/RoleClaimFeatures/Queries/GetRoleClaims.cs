using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Application.Features.RoleFeatures.RoleClaimFeatures.Models;
using System.ComponentModel;
using Utils.CustomAttributes;

namespace Application.Features.RoleFeatures.RoleClaimFeatures.Queries;

public class GetRoleClaimsQuery : IBaseRequest<IEnumerable<RoleClaimDto>>
{
    [RequiredFa, DisplayName("آیدی نقش")]
    public int RoleId { get; set; }
}

internal class GetRoleClaimsHandler(IUnitOfWork uow) : IBaseHandler<GetRoleClaimsQuery,IEnumerable<RoleClaimDto>>
{
    public async Task<ApiResult<IEnumerable<RoleClaimDto>>> Handle(GetRoleClaimsQuery request, CancellationToken cancellationToken)
    {
        var claims = await uow.Roles.GetClaimsByRoleIdAsync(request.RoleId, cancellationToken);
        return ApiResult<IEnumerable<RoleClaimDto>>.Success(claims);
    }
}