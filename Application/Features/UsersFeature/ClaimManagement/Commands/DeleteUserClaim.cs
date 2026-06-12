using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using System.ComponentModel;
using Utils.CustomAttributes;

namespace Application.Features.UsersFeature.ClaimManagement.Commands;

public class DeleteUserClaimCommand : IBaseRequest
{
    [RequiredFa, DisplayName("آیدی")]
    public int Id { get; set; }
}

internal class DeleteUserClaimHandler(IUnitOfWork uow) : IBaseHandler<DeleteUserClaimCommand>
{
    public async Task<ApiResult> Handle(DeleteUserClaimCommand request, CancellationToken cancellationToken)
    {
        var claim = await uow.Users.GetClaimById(request.Id, cancellationToken);

        if (claim is null)
            return ApiResult.NotFound("کلیم");

        uow.Users.DeleteClaim(claim);
        var res = await uow.SaveChangesAsync(cancellationToken);

        return res.ToApiResult();
    }
}