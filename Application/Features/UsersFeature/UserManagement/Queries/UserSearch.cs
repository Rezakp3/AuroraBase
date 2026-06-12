using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Application.Common.Models.Pagination;
using Application.Features.UsersFeature.UserManagement.Models;

namespace Application.Features.UsersFeature.UserManagement.Queries;

public class UserSearchQuery
    : UserIm, IBaseRequest<PaginatedList<UserDto>>;

internal class UserSearchHandler(IUnitOfWork uow)
    : IBaseHandler<UserSearchQuery, PaginatedList<UserDto>>
{
    public async Task<ApiResult<PaginatedList<UserDto>>> Handle(UserSearchQuery request, CancellationToken cancellationToken)
    {
        var data = await uow.Users.Search(request, cancellationToken);

        return ApiResult<PaginatedList<UserDto>>.Success(data);
    }
}