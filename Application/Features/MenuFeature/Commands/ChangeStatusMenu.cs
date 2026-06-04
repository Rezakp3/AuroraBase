using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Utils.CustomAttributes;

namespace Application.Features.MenuFeature.Commands;

public class ChangeStatusMenuCommand : IBaseRequest
{
    [RequiredFa, DisplayName("آیدی")]
    public int Id { get; set; }

    [RequiredFa, DisplayName("وضعیت")]
    public bool IsActive { get; set; }
}

internal class ChangeStatusMenuHandler(IUnitOfWork uow) 
    : IBaseHandler<ChangeStatusMenuCommand>
{
    public async Task<ApiResult> Handle(ChangeStatusMenuCommand request, CancellationToken cancellationToken)
    {
        var menu = await uow.Menus.GetByIdAsync(request.Id, cancellationToken);
        if (menu is null)
            return ApiResult.NotFound("منو");
        menu.IsActive = request.IsActive;
        var res = await uow.SaveChangesAsync(cancellationToken);
        return res.ToApiResult();
    }
}
