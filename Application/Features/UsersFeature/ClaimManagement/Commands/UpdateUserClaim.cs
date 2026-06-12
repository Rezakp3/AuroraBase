using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Core.Entities.Auth;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.UsersFeature.ClaimManagement.Commands;

public class UpdateUserClaimCommand : IBaseRequest
{
    public int Id { get; set; }
    public string Type { get; set; }
    public string Value { get; set; }
}

internal class UpdateUserClaimHandler(IUnitOfWork uow) : IBaseHandler<UpdateUserClaimCommand>
{
    public async Task<ApiResult> Handle(UpdateUserClaimCommand request, CancellationToken cancellationToken)
    {
        var claim = await uow.Users.GetClaimById(request.Id, cancellationToken);

        if (claim is null)
            return ApiResult.NotFound("کلیم");

        claim.ClaimType = request.Type; 
        claim.ClaimValue = request.Value;

        var res = await uow.SaveChangesAsync(cancellationToken);

        return res.ToApiResult();
    }
}