using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Application.Common.Models.Pagination;
using Application.Features.RoleFeatures.RoleManagement.Models;

namespace Application.Features.RoleFeatures.RoleManagement.Queries;

public class SearchRoleQuery : RoleIm, IBaseRequest<PaginatedList<RoleDto>>;

internal class SearchRoleHandler(IUnitOfWork uow) : IBaseHandler<SearchRoleQuery, PaginatedList<RoleDto>>
{
    public async Task<ApiResult<PaginatedList<RoleDto>>> Handle(SearchRoleQuery request, CancellationToken cancellationToken)
    {
        var res = await uow.Roles.Search(request, cancellationToken);
        return ApiResult<PaginatedList<RoleDto>>.Success(res);
    }
}