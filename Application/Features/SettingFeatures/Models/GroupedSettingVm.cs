using Core.Enums;
using Utils.Helpers;

namespace Application.Features.SettingFeatures.Models;

public class GroupedSettingVm
{
    public string Group { get; set; }
    public IEnumerable<SettingVm> Settings { get; set; }
}

public class SettingVm
{
    public int Id { get; set; }
    public string Key { get; set; }
    public string Value { get; set; }
    public string Description { get; set; }
    public ESettingDataType DataType { get; set; }
    public string DataTypeStr => DataType.GetDescription();
}
