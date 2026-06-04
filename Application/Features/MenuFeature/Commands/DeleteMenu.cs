using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Utils.CustomAttributes;

namespace Application.Features.MenuFeature.Commands;

public class DeleteMenuCommand : IBaseRequest
{
    [RequiredFa, DisplayName("آیدی")]
    public int Id { get; set; }
}

internal class DeleteMenuHandler(IUnitOfWork uow) : IBaseHandler<DeleteMenuCommand>
{
    public async Task<ApiResult> Handle(DeleteMenuCommand request, CancellationToken cancellationToken)
    {
        var menu = await uow.Menus.GetByIdAsync(request.Id, cancellationToken);
        if (menu is null)
            return ApiResult.NotFound("منو");
        uow.Menus.Delete(menu);
        var res = await uow.SaveChangesAsync(cancellationToken);
        return res.ToApiResult();
    }
}
