using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Application.Common.Models.Pagination;

namespace Application.Features.RoleFeatures.RoleManagement.Queries;

public class RoleDropDownQuery
    : CursorPagingOption<int>, IBaseRequest<CursorPaginatedList<BaseDropDown<int>, int>>
{
    public string Name { get; set; }
}

internal class RoleDropDownHandler(IUnitOfWork uow)
    : IBaseHandler<RoleDropDownQuery, CursorPaginatedList<BaseDropDown<int>, int>>
{
    public async Task<ApiResult<CursorPaginatedList<BaseDropDown<int>, int>>> Handle(RoleDropDownQuery request, CancellationToken cancellationToken)
    {
        var data = await uow.Roles.DropDown(request.Name, request, cancellationToken);

        return ApiResult<CursorPaginatedList<BaseDropDown<int>, int>>.Success(data);
    }
}