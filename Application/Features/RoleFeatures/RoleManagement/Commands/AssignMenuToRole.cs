using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using System.ComponentModel;
using Utils.CustomAttributes;

namespace Application.Features.RoleFeatures.RoleManagement.Commands;

public class AssignMenuToRoleCommand : IBaseRequest
{
    [RequiredFa, DisplayName("آیدی")]
    public int RoleId { get; set; }

    [RequiredFa, DisplayName("سرویس")]
    public IEnumerable<int> Menues { get; set; }
}

internal class AssignMenuToRoleHandler(IUnitOfWork uow) : IBaseHandler<AssignMenuToRoleCommand>
{
    public async Task<ApiResult> Handle(AssignMenuToRoleCommand request, CancellationToken cancellationToken)
    {
        var roleExist = await uow.Roles.AnyAsync(x => x.Id == request.RoleId, cancellationToken);
        if (!roleExist)
            return ApiResult.NotFound("نقش");

        await uow.Roles.AssignMenuesToRole(request.RoleId, request.Menues, cancellationToken);

        var res = await uow.SaveChangesAsync(cancellationToken); 

        return res.ToApiResult();
    }
}