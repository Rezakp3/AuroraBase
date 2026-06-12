using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using System.ComponentModel;
using Utils.CustomAttributes;

namespace Application.Features.RoleFeatures.RoleRelations.Commands;

public class ManageRoleMenusCommand : IBaseRequest
{
    [RequiredFa, DisplayName("آیدی")]
    public int RoleId { get; set; }

    [RequiredFa, DisplayName("سرویس")]
    public IEnumerable<int> Menues { get; set; }
}

internal class ManageRoleMenusHandler(IUnitOfWork uow)
    : IBaseHandler<ManageRoleMenusCommand>
{
    public async Task<ApiResult> Handle(ManageRoleMenusCommand request, CancellationToken ct)
    {
        var roleExist = await uow.Roles.AnyAsync(x => x.Id == request.RoleId, ct);
        if (!roleExist)
            return ApiResult.NotFound("نقش");

        await uow.BeginTransactionAsync(ct);

        try
        {
            await uow.Roles.DeleteMenues(request.RoleId, ct);

            await uow.Roles.AddRangeMenues(request.RoleId, request.Menues, ct);

            var res = await uow.SaveChangesAsync(ct);

            await uow.CommitTransactionAsync(ct);

            return res.ToApiResult();
        }
        catch 
        {
            await uow.RollbackTransactionAsync(ct);
            throw;
        }
    }
}