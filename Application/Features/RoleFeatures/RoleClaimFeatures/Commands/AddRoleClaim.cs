using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Core.Entities.Auth;
using System.ComponentModel;
using Utils.CustomAttributes;

namespace Application.Features.RoleFeatures.RoleClaimFeatures.Commands;

public class AddRoleClaimCommand : IBaseRequest
{
    [RequiredFa, DisplayName("نقش")]
    public int RoleId { get; set; }
    [RequiredFa, DisplayName("تایپ"), MaxLengthFa(10)]
    public string Type { get; set; }
    [RequiredFa, DisplayName("مقدار"), MaxLengthFa(30)]
    public string Value { get; set; }
}

internal class AddRoleClaimHandler(IUnitOfWork uow) : IBaseHandler<AddRoleClaimCommand>
{
    public async Task<ApiResult> Handle(AddRoleClaimCommand request, CancellationToken cancellationToken)
    {
        var claim = new RoleClaim
        {
            RoleId = request.RoleId,
            Type = request.Type,
            Value = request.Value
        };

        uow.Roles.AddClaim(claim,cancellationToken);

        var res = await uow.SaveChangesAsync(cancellationToken);

        return res.ToApiResult();
    }
}