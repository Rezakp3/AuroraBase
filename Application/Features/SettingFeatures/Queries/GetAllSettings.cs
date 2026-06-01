using Application.Features.SettingFeatures.Models;
using Application.Common.Interfaces.Generals;
using Application.Common.Models;

namespace Application.Features.SettingFeatures.Queries;

public class GetAllSettingsQuery : IBaseRequest<IEnumerable<GroupedSettingVm>>;

internal class GetAllSettingsHandler(IUnitOfWork uow) : IBaseHandler<GetAllSettingsQuery, IEnumerable<GroupedSettingVm>>
{
    public async Task<ApiResult<IEnumerable<GroupedSettingVm>>> Handle
        (GetAllSettingsQuery request, CancellationToken cancellationToken)
    {
        var list = await uow.Settings.GetAllAsync(cancellationToken);

        var groupedList = list
            .GroupBy(x => x.Group)
            .Select(x => new GroupedSettingVm()
            {
                Group = x.Key,
                Settings = x.Select(c => new SettingVm
                {
                    Id = c.Id,
                    DataType = c.DataType,
                    Description = c.Description,
                    Key = c.Key,
                    Value = c.Value
                })
            });

        return ApiResult<IEnumerable<GroupedSettingVm>>.Success(groupedList);
    }
}