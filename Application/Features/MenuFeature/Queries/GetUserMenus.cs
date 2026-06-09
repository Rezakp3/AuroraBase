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

public record GetUserMenusQuery : IBaseRequest<IEnumerable<MenuDto>>;

internal class GetUserMenusQueryHandler(IUnitOfWork uow, IHttpContextAccessor accessor) : IBaseHandler<GetUserMenusQuery, IEnumerable<MenuDto>>
{
    public async Task<ApiResult<IEnumerable<MenuDto>>> Handle(GetUserMenusQuery request, CancellationToken cancellationToken)
    {
        var res = new ApiResult<IEnumerable<MenuDto>>();
        var userId = accessor.GetUserId<long>();

        var data = await uow.Menus.GetByUserId(userId, cancellationToken);

        if(data is null)
            return res.NotFound("منویی");

        var mappedData = data.Where(x => x.ParentId is null).Select(x => Map(x, data));
        return res.Success(mappedData);
    }

    private static MenuDto Map(Menu menu, IEnumerable<Menu> all)
        => new()
        {
            Id = menu.Id,
            ParentId = menu.ParentId,
            Route = menu.Route,
            Title = menu.Title,
            IsActive = menu.IsActive,
            Icon = menu.Icon,
            Priority = menu.Priority,
            SubMenus = [.. all.Where(x => x.ParentId == menu.Id).Select(x => Map(x, all))],
        };
}