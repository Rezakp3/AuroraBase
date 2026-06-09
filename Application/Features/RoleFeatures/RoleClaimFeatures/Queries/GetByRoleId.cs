using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Application.Features.RoleFeatures.RoleClaimFeatures.Models;
using System.ComponentModel;
using Utils.CustomAttributes;

namespace Application.Features.RoleFeatures.RoleClaimFeatures.Queries;

public class GetByRoleIdQuery : IBaseRequest<IEnumerable<RoleClaimDto>>
{
    [RequiredFa, DisplayName("آیدی نقش")]
    public int RoleId { get; set; }
}

internal class GetByRoleIdHandler(IUnitOfWork uow) : IBaseHandler<GetByRoleIdQuery,IEnumerable<RoleClaimDto>>
{
    public async Task<ApiResult<IEnumerable<RoleClaimDto>>> Handle(GetByRoleIdQuery request, CancellationToken cancellationToken)
    {
        var claims = await uow.Roles.GetClaimsByRoleIdAsync(request.RoleId, cancellationToken);
        return ApiResult<IEnumerable<RoleClaimDto>>.Success(claims);
    }
}