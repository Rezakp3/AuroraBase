using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Application.Common.Models.Pagination;
using System.ComponentModel;
using Utils.CustomAttributes;

namespace Application.Features.RoleFeatures.RoleRelations.Queries;

public class RoleServiceDropDownQuery :IBaseRequest<IEnumerable<BaseDropDown<int>>> 
{
    [RequiredFa,DisplayName("نقش")]
    public int RoleId { get; set; }
    public string Search { get; set; }
}

internal class RoleServiceDropDownHandler(IUnitOfWork uow)
    : IBaseHandler<RoleServiceDropDownQuery, IEnumerable<BaseDropDown<int>>>
{
    public async Task<ApiResult<IEnumerable<BaseDropDown<int>>>> Handle(RoleServiceDropDownQuery request, CancellationToken cancellationToken)
    {
        var services = await uow.Roles.ServiceDropDown
            (request.RoleId, request.Search, cancellationToken);

        return ApiResult<IEnumerable<BaseDropDown<int>>>.Success(services);
    }
}