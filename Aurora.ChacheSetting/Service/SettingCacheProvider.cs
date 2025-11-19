// فایل: MyProject.Core/Interfaces/ConcurrentSettingCacheProvider.cs

using System.Collections.Concurrent;

namespace Aurora.ChacheSetting.Service;

// پیاده‌سازی نهایی ISettingCacheProvider
public class SettingCacheProvider : ISettingCacheProvider
{
    private readonly ConcurrentDictionary<string, Dictionary<string, SettingModel>> _settingsCache = new();

    #region Command & Write Operations

    public bool Set(string key, SettingModel setting, string group = "General")
    {
        // AddOrUpdate یک عملیات اتمیک روی سطح ConcurrentDictionary انجام می‌دهد.
        _settingsCache.AddOrUpdate(
            group,
            new Dictionary<string, SettingModel> { { key, setting } }, // مقدار اگر گروه وجود نداشت
            (k, existingDict) =>
            {
                // این قفل به طور موقت از تغییر دیکشنری داخلی جلوگیری می‌کند 
                // در حالی که یک مقدار در آن تغییر داده می‌شود (برای Thread Safety در دیکشنری تو در تو).
                lock (existingDict)
                {
                    existingDict[key] = setting;
                }
                return existingDict;
            });
        return true;
    }
    public bool SetGroup(string group, Dictionary<string, SettingModel> settings)
    {
        // جایگزینی کل گروه به صورت اتمیک
        _settingsCache[group] = settings;
        return true;
    }
    public bool RemoveGroup(string group)
    {
        return _settingsCache.TryRemove(group, out _);
    }
    public bool ClearAll()
    {
        _settingsCache.Clear();
        return true;
    }
    public bool UpdateValue(string key, string newValue, string group = "General")
    {
        if (_settingsCache.TryGetValue(group, out var cachedGroup))
        {
            lock (cachedGroup)
            {
                if (cachedGroup.TryGetValue(key, out var setting))
                {
                    // به‌روزرسانی مقدار (توجه: چون SettingModel یک record است، در اینجا مقدار داخلی آن تغییر می‌کند)
                    // اگر Setting یک record بود که Immutability را تضمین می‌کرد، باید یک کپی جدید می‌ساختیم و دیکشنری داخلی را جایگزین می‌کردیم.
                    // فرض می‌کنیم در اینجا تغییر مستقیم مقدار مجاز است.

                    // به دلیل record بودن، بهتر است کپی بسازیم و دیکشنری داخلی را به‌روز کنیم:
                    var newSetting = setting with { Value = newValue, Group = group, Key = key };
                    cachedGroup[key] = newSetting;

                    return true;
                }
            }
        }
        return false;
    }

    #endregion

    #region Read & Existence Operations

    
    public Dictionary<string, SettingModel> GetGroup(string group)
    {
        // در صورت عدم وجود، یک دیکشنری خالی برگردان
        if (_settingsCache.TryGetValue(group, out var cachedGroup))
        {
            // برای جلوگیری از تغییر تصادفی توسط مصرف‌کننده، کپی برگردانده می‌شود.
            return new Dictionary<string, SettingModel>(cachedGroup);
        }
        return new Dictionary<string, SettingModel>();
    }
    public List<SettingModel> GetByGroup(string group = "General")
    {
        if (_settingsCache.TryGetValue(group, out var cachedGroup))
        {
            return cachedGroup.Values.ToList();
        }
        return new List<SettingModel>();
    }
    public SettingModel GetByKey(string key, string group = "General")
    {
        if (_settingsCache.TryGetValue(group, out var cachedGroup))
        {
            if (cachedGroup.TryGetValue(key, out var setting))
            {
                return setting;
            }
        }
        // در صورت عدم یافتن، مقدار دیفالت (null) برگردانده می‌شود.
        return default!;
    }    
    public List<SettingModel> GetAllSettings()
    {
        // جمع‌آوری مقادیر از تمام گروه‌ها
        return _settingsCache.Values.SelectMany(d => d.Values).ToList();
    }
    public List<string> GetAllGroups()
    {
        return _settingsCache.Keys.ToList();
    }
    public bool GroupExists(string group)
    {
        // توجه: این متد در اینترفیس تکرار شده بود، فقط یک بار پیاده‌سازی می‌شود.
        return _settingsCache.ContainsKey(group);
    }
    public bool KeyExists(string key, string group = "General")
    {
        if (_settingsCache.TryGetValue(group, out var cachedGroup))
        {
            return cachedGroup.ContainsKey(key);
        }
        return false;
    }


    #endregion
}