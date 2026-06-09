using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using System.ComponentModel;
using Utils.CustomAttributes;

namespace Application.Features.MenuFeature.Commands;

public class UpdateMenuCommand : IBaseRequest
{
    [RequiredFa, DisplayName("آیدی")]
    public int Id { get; set; }
    [RequiredFa, DisplayName("تیتر"), MaxLengthFa(50)]
    public string Title { get; set; }
    [RequiredFa, DisplayName("آدرس"), MaxLengthFa(150)]
    public string Route { get; set; }
    [DisplayName("آیکون"), MaxLengthFa(30)]
    public string Icon { get; set; }
    public int? ParentId { get; set; }

    public int Priority { get; set; }
    public bool IsActive { get; set; } = true;
}

internal class UpdateMenuHandler(IUnitOfWork uow) : IBaseHandler<UpdateMenuCommand>
{
    public async Task<ApiResult> Handle(UpdateMenuCommand request, CancellationToken cancellationToken)
    {
        var menu = await uow.Menus.GetByIdAsync(request.Id, cancellationToken);
        if (menu is null)
            return ApiResult.NotFound("منو");

        menu.Title = request.Title;
        menu.Route = request.Route;
        menu.ParentId = request.ParentId;
        menu.Priority = request.Priority;
        menu.Icon = request.Icon;
        menu.IsActive = request.IsActive;

        var res = await uow.SaveChangesAsync(cancellationToken);
        return res.ToApiResult();
    }
}