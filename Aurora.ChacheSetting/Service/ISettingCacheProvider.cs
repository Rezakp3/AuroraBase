namespace Aurora.ChacheSetting.Service;

/// <summary>
/// قرارداد خالص برای ذخیره و واکشی کش تنظیمات.
/// </summary>
public record SettingModel(string Group, string Key, string Value);
public interface ISettingCacheProvider
{
    bool Set(string key, SettingModel setting, string group = "General");
    Dictionary<string, SettingModel> GetGroup(string group);
    bool SetGroup(string group, Dictionary<string, SettingModel> settings);
    bool RemoveGroup(string group);
    bool ClearAll();
    bool GroupExists(string group);
    List<SettingModel> GetByGroup(string group = "General" );
    SettingModel GetByKey(string key, string group = "General" );
    List<SettingModel> GetAllSettings();
    bool UpdateValue(string key, string newValue, string group = "General" );
    bool KeyExists(string key, string group = "General" );
    List<string> GetAllGroups();
}
