using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Application.Common.Models.Pagination;

namespace Application.Features.MenuFeature.MenuManagement.Queries;

public class MenuDropDownQuery : CursorPagingOption<int>
    , IBaseRequest<CursorPaginatedList<BaseDropDown<int>, int>>
{
    public string Title { get; set; }
}

internal class MenuDropDownHandler(IUnitOfWork uow)
    : IBaseHandler<MenuDropDownQuery, CursorPaginatedList<BaseDropDown<int>, int>>
{
    public async Task<ApiResult<CursorPaginatedList<BaseDropDown<int>, int>>> Handle(MenuDropDownQuery request, CancellationToken cancellationToken)
    {
        var data = await uow.Menus.DropDown(request.Title, request, cancellationToken);

        return ApiResult<CursorPaginatedList<BaseDropDown<int>, int>>.Success(data);
    }
}