using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using System.ComponentModel;
using Utils.CustomAttributes;

namespace Application.Features.MenuFeature.MenuRelations.Queries;

public class MenuRoleDropDownQuery
    : IBaseRequest<IEnumerable<BaseDropDown<int>>>
{
    [RequiredFa, DisplayName("منو")]
    public int MenuId { get; set; }
    public string RoleTitle { get; set; }
}

internal class MenuRoleDropDownHandler(IUnitOfWork uow)
    : IBaseHandler<MenuRoleDropDownQuery, IEnumerable<BaseDropDown<int>>>
{
    public async Task<ApiResult<IEnumerable<BaseDropDown<int>>>> Handle(MenuRoleDropDownQuery request, CancellationToken cancellationToken)
    {
        var data = await uow.Menus.MenuRolesDropDown(request.MenuId,request.RoleTitle, cancellationToken);

        return ApiResult<IEnumerable<BaseDropDown<int>>>.Success(data);
    }
}