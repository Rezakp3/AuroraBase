using Application.Common.Interfaces.Generals;
using Application.Common.Models;

namespace Application.Features.UsersFeature.UserRelations.Commands;

public class ManageUserRoleCommand : IBaseRequest
{
    public long UserId { get; set; }
    public IEnumerable<int> Roles { get; set; }
}

internal class ManageUserRoleHandler(IUnitOfWork uow)
    : IBaseHandler<ManageUserRoleCommand>
{
    public async Task<ApiResult> Handle(ManageUserRoleCommand request, CancellationToken cancellationToken)
    {
        await uow.UserRoles.SyncUserRolesAsync(request.UserId,request.Roles,cancellationToken);

        var res = await uow.SaveChangesAsync(cancellationToken);

        return res.ToApiResult();
    }
}