using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using Core.Entities.Auth;
using Mapster;
using System.ComponentModel;
using Utils.CustomAttributes;

namespace Application.Features.MenuFeature.Commands;

public class AddMenuCommand : IBaseRequest
{
    [RequiredFa, DisplayName("تیتر"), MaxLengthFa(50)]
    public string Title { get; set; }
    [RequiredFa, DisplayName("آدرس"), MaxLengthFa(150)]
    public string Route { get; set; }
    public int? ParentId { get; set; }

    public int Priority { get; set; }
    public bool IsActive { get; set; } = true;
}

internal class AddMenuHandler(IUnitOfWork uow) : IBaseHandler<AddMenuCommand>
{
    public async Task<ApiResult> Handle(AddMenuCommand request, CancellationToken cancellationToken)
    {
        var menu = request.Adapt<Menu>();

        await uow.Menus.AddAsync(menu,cancellationToken);

        var res = await uow.SaveChangesAsync(cancellationToken);

        return res.ToApiResult();
    }
}