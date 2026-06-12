using Application.Common.Interfaces.Generals;
using Application.Common.Models;

namespace Application.Features.UsersFeature.UserRelations.Queries;

public class UserRoleDropDownQuery : IBaseRequest<IEnumerable<BaseDropDown<int>>>
{
    public long UserId { get; set; }
    public string Search { get; set; }
}

internal class UserRoleDropDownHandler(IUnitOfWork uow)
    : IBaseHandler<UserRoleDropDownQuery, IEnumerable<BaseDropDown<int>>>
{
    public async Task<ApiResult<IEnumerable<BaseDropDown<int>>>> Handle(UserRoleDropDownQuery request, CancellationToken cancellationToken)
    {
        var data = await uow.Users.UserRolesDropDown(request.UserId, request.Search, cancellationToken);

        return ApiResult<IEnumerable<BaseDropDown<int>>>.Success(data);
    }
}