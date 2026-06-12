using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Application.Features.MenuFeature.MenuManagement.Models;
using Aurora.Jwt.Helpers;
using Microsoft.AspNetCore.Http;

namespace Application.Features.MenuFeature.MenuManagement.Queries;

public class SideBarMenuQuery : IBaseRequest<IEnumerable<MenuDto>>;

internal class SideBarMenuHandler(IUnitOfWork uow, IHttpContextAccessor accessor)
    : IBaseHandler<SideBarMenuQuery, IEnumerable<MenuDto>>
{
    public async Task<ApiResult<IEnumerable<MenuDto>>> Handle(SideBarMenuQuery request, CancellationToken cancellationToken)
    {
        var userid = accessor.GetUserId<long>();

        var userRoles = await uow.UserRoles.GetUserRoleIdsAsync(userid, cancellationToken);

        var menus = await uow.Menus.GetMenusByRoleIds(userRoles, cancellationToken);

        return ApiResult<IEnumerable<MenuDto>>.Success(menus);
    }
}