using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using System.ComponentModel;
using Utils.CustomAttributes;

namespace Application.Features.RoleFeatures.RoleManagement.Commands;

public class AssignServicesToRoleCommand : IBaseRequest
{

    [RequiredFa, DisplayName("آیدی")]
    public int Id { get; set; }

    public IEnumerable<int> ServiceIds { get; set; }
}

internal class AssignServicesToRoleHandler(IUnitOfWork uow) : IBaseHandler<AssignServicesToRoleCommand>
{
    public async Task<ApiResult> Handle(AssignServicesToRoleCommand request, CancellationToken cancellationToken)
    {
        var roleExist = await uow.Roles.AnyAsync(x => x.Id == request.Id, cancellationToken);
        if (!roleExist)
            return ApiResult.NotFound("نقش");

        await uow.RoleServices.SyncPermissionsAsync(request.Id, request.ServiceIds, cancellationToken);

        var res = await uow.SaveChangesAsync(cancellationToken);

        return res.ToApiResult();
    }
}