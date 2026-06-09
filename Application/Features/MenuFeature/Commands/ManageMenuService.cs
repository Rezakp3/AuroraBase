using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using System.ComponentModel;
using Utils.CustomAttributes;

namespace Application.Features.MenuFeature.Commands;

public class ManageMenuServiceCommand : IBaseRequest
{
    [RequiredFa, DisplayName("منو")]
    public int MenuId { get; set; }
    public IEnumerable<int> ServiceIds { get; set; }
}

internal class ManageMenuServiceHandler(IUnitOfWork uow)
    : IBaseHandler<ManageMenuServiceCommand>
{
    public async Task<ApiResult> Handle(ManageMenuServiceCommand request, CancellationToken cancellationToken)
    {
        await uow.BeginTransactionAsync(cancellationToken);

        try
        {
            await uow.Menus.DeleteMenuServicesByMenuId(request.MenuId, cancellationToken);

            await uow.Menus.AddRangeServices(request.MenuId, request.ServiceIds, cancellationToken);

            var res = await uow.SaveChangesAsync(cancellationToken);

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