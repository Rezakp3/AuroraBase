using Application.Common.Interfaces.Generals;
using Application.Common.Models;
using System.ComponentModel;
using Utils.CustomAttributes;

namespace Application.Features.SettingFeatures.Commands;

public class UpdateSettingCommand : IBaseRequest
{
    [RequiredFa, DisplayName("آیدی")]
    public int Id { get; set; }
    [RequiredFa, DisplayName("مقدار")]
    public string Value { get; set; }
}

internal class UpdateSettingHandler(IUnitOfWork uow) : IBaseHandler<UpdateSettingCommand>
{
    public async Task<ApiResult> Handle(UpdateSettingCommand request, CancellationToken cancellationToken)
    {
        var entity = await uow.Settings.GetByIdAsync(request.Id, cancellationToken);

        if (entity is null)
            return ApiResult.NotFound("رکورد");

        entity.Value = request.Value;

        await uow.Settings.UpdateValueAsync(entity.Key, entity.Value, entity.Group, cancellationToken);

        var res = await uow.SaveChangesAsync(cancellationToken);

        return res.ToApiResult();
    }
}