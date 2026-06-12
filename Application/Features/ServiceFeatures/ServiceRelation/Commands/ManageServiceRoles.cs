using Application.Common.Interfaces.Generals;
using Application.Common.Models;

namespace Application.Features.ServiceFeatures.ServiceRelation.Commands;

public class ManageServiceRolesCommand : IBaseRequest
{
    public int ServiceId { get; set; }

    public IEnumerable<int> RoleIds { get; set; }
}

internal class ManageServiceRolesHandler(IUnitOfWork uow) : IBaseHandler<ManageServiceRolesCommand>
{
    public async Task<ApiResult> Handle(ManageServiceRolesCommand request, CancellationToken cancellationToken)
    {
        await uow.RoleServices.SyncRolesForServiceAsync(request.ServiceId, request.RoleIds, cancellationToken);

        var res = await uow.SaveChangesAsync(cancellationToken);

        return res.ToApiResult();
    }
}