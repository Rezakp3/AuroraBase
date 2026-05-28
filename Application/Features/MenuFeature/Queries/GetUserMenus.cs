using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Application.Features.MenuFeature.Models;
using Aurora.Jwt.Helpers;
using Core.Entities.Auth;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.MenuFeature.Queries;

public record GetUserMenusQuery : IBaseRequest<IEnumerable<MenuVm>>;

internal class GetUserMenusQueryHandler(IUnitOfWork uow, IHttpContextAccessor accessor) : IBaseHandler<GetUserMenusQuery, IEnumerable<MenuVm>>
{
    public async Task<ApiResult<IEnumerable<MenuVm>>> Handle(GetUserMenusQuery request, CancellationToken cancellationToken)
    {
        var res = new ApiResult<IEnumerable<MenuVm>>();
        var userId = accessor.GetUserId<long>();

        var data = await uow.Menus.GetByUserId(userId, cancellationToken);

        if(data is null)
            return res.NotFound("منویی");

        var mappedData = data.Where(x => x.ParentId is null).Select(x => Map(x, data));
        return res.Success(mappedData);
    }

    private static MenuVm Map(Menu menu, IEnumerable<Menu> all)
        => new()
        {
            Id = menu.Id,
            ParentId = menu.ParentId,
            Route = menu.Route,
            Title = menu.Title,
            SubMenus = [.. all.Where(x => x.ParentId == menu.Id).Select(x => Map(x, all))],
        };
}