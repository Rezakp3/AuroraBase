using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using System.ComponentModel;
using Utils.CustomAttributes;

namespace Application.Features.RoleFeatures.RoleRelations.Queries;

public class RoleMenuDropDownQuery : IBaseRequest<IEnumerable<BaseDropDown<int>>>
{
    [RequiredFa, DisplayName("نقش")]
    public int RoleId { get; set; }
    public string Search { get; set; }
}

internal class RoleMenuDropDownHandler(IUnitOfWork uow)
    : IBaseHandler<RoleMenuDropDownQuery, IEnumerable<BaseDropDown<int>>>
{
    public async Task<ApiResult<IEnumerable<BaseDropDown<int>>>> Handle(RoleMenuDropDownQuery request, CancellationToken cancellationToken)
    {
        var menus = await uow.Roles.MenuDropDown(request.RoleId,request.Search,cancellationToken);

        return ApiResult<IEnumerable<BaseDropDown<int>>>.Success(menus);
    }
}