using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Application.Common.Models.Pagination;
using System.ComponentModel;
using Utils.CustomAttributes;

namespace Application.Features.MenuFeature.MenuRelations.Queries;

public class MenuServiceDropDownQuery : IBaseRequest<IEnumerable<BaseDropDown<int>>>
{
    [RequiredFa, DisplayName("منو")]
    public int MenuId { get; set; }
    public string ServiceName { get; set; }
}

internal class MenuServiceDropDownHandler(IUnitOfWork uow)
    : IBaseHandler<MenuServiceDropDownQuery, IEnumerable<BaseDropDown<int>>>
{
    public async Task<ApiResult<IEnumerable<BaseDropDown<int>>>> Handle(MenuServiceDropDownQuery request, CancellationToken cancellationToken)
    {
        var data = await uow.Menus.MenuServicesDropDown(request.MenuId, request.ServiceName, cancellationToken);

        return ApiResult<IEnumerable<BaseDropDown<int>>>.Success(data);
    }
}