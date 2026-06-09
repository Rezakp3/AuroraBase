using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Application.Common.Models.Pagination;
using System.ComponentModel;
using Utils.CustomAttributes;

namespace Application.Features.MenuFeature.Queries;

public class MenuRoleDropDownQuery
    : CursorPagingOption<int>, IBaseRequest<CursorPaginatedList<BaseDropDown<int>, int>>
{
    [RequiredFa, DisplayName("منو")]
    public int MenuId { get; set; }
    public string RoleTitle { get; set; }
}

internal class MenuRoleDropDownHandler(IUnitOfWork uow)
    : IBaseHandler<MenuRoleDropDownQuery, CursorPaginatedList<BaseDropDown<int>, int>>
{
    public async Task<ApiResult<CursorPaginatedList<BaseDropDown<int>, int>>> Handle(MenuRoleDropDownQuery request, CancellationToken cancellationToken)
    {
        var data = await uow.Menus.MenuRolesDropDown(request.MenuId,request.RoleTitle, request, cancellationToken);

        return ApiResult<CursorPaginatedList<BaseDropDown<int>,int>>.Success(data);
    }
}