using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Application.Features.UsersFeature.ClaimManagement.Models;
using System.ComponentModel;
using Utils.CustomAttributes;

namespace Application.Features.UsersFeature.ClaimManagement.Queries;

public class GetUserClaimsQuery : IBaseRequest<IEnumerable<UserClaimDto>>
{
    [RequiredFa, DisplayName("کاربر")]
    public long UserId { get; set; }
}

internal class GetUserClaimsHandler(IUnitOfWork uow) 
    : IBaseHandler<GetUserClaimsQuery, IEnumerable<UserClaimDto>>
{
    public async Task<ApiResult<IEnumerable<UserClaimDto>>> Handle(GetUserClaimsQuery request, CancellationToken cancellationToken)
    {
        var claims = await uow.Users.GetClaims(request.UserId, cancellationToken);

        return ApiResult<IEnumerable<UserClaimDto>>.Success(claims);
    }
}