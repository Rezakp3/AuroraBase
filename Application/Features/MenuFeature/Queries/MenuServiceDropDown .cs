using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Application.Common.Models.Pagination;
using System.ComponentModel;
using Utils.CustomAttributes;

namespace Application.Features.MenuFeature.Queries;

public class MenuServiceDropDownQuery
    : CursorPagingOption<int>, IBaseRequest<CursorPaginatedList<BaseDropDown<int>, int>>
{
    [RequiredFa, DisplayName("منو")]
    public int MenuId { get; set; }
    public string ServiceName { get; set; }
}

internal class MenuServiceDropDownHandler(IUnitOfWork uow)
    : IBaseHandler<MenuServiceDropDownQuery, CursorPaginatedList<BaseDropDown<int>, int>>
{
    public async Task<ApiResult<CursorPaginatedList<BaseDropDown<int>, int>>> Handle(MenuServiceDropDownQuery request, CancellationToken cancellationToken)
    {
        var data = await uow.Menus.MenuServicesDropDown(request.MenuId, request.ServiceName, request, cancellationToken);

        return ApiResult<CursorPaginatedList<BaseDropDown<int>, int>>.Success(data);
    }
}