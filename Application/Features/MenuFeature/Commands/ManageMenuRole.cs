using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using System.ComponentModel;
using Utils.CustomAttributes;

namespace Application.Features.MenuFeature.Commands;

public class ManageMenuRoleCommand : IBaseRequest
{
    [RequiredFa, DisplayName("منو")]
    public int MenuId { get; set; }
    public IEnumerable<int> Roles { get; set; }
}

internal class ManageMenuRoleHandler(IUnitOfWork uow) : IBaseHandler<ManageMenuRoleCommand>
{
    public async Task<ApiResult> Handle(ManageMenuRoleCommand request, CancellationToken cancellationToken)
    {
        await uow.BeginTransactionAsync(cancellationToken);

        try
        {
            await uow.Menus.DeleteMenuRolesByMenuId(request.MenuId, cancellationToken);

            await uow.Menus.AddRangeRoles(request.MenuId, request.Roles, cancellationToken);

            await uow.SaveChangesAsync(cancellationToken);

            await uow.CommitTransactionAsync(cancellationToken);

            return ApiResult.Success();
        }
        catch
        {
            await uow.RollbackTransactionAsync(cancellationToken);

            throw;
        }
    }
}