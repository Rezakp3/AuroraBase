using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using System.ComponentModel;
using Utils.CustomAttributes;

namespace Application.Features.RoleFeatures.RoleRelations.Commands;

public class ManageRoleServiceCommand : IBaseRequest
{

    [RequiredFa, DisplayName("آیدی")]
    public int Id { get; set; }

    public IEnumerable<int> ServiceIds { get; set; }
}

internal class ManageRoleServiceHandler(IUnitOfWork uow) : IBaseHandler<ManageRoleServiceCommand>
{
    public async Task<ApiResult> Handle(ManageRoleServiceCommand request, CancellationToken cancellationToken)
    {
        var roleExist = await uow.Roles.AnyAsync(x => x.Id == request.Id, cancellationToken);
        if (!roleExist)
            return ApiResult.NotFound("نقش");

        await uow.BeginTransactionAsync(cancellationToken);

        try
        {
            await uow.RoleServices.SyncPermissionsAsync(request.Id, request.ServiceIds, cancellationToken);


            var res = await uow.SaveChangesAsync(cancellationToken);
            await uow.CommitTransactionAsync(cancellationToken);

            return res.ToApiResult();
        }
        catch 
        {
            await uow.RollbackTransactionAsync(cancellationToken);
            throw;
        }

    }
}