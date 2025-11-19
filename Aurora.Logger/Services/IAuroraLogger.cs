namespace Aurora.Logger.Services;

public interface IAuroraLogger
{
    // متدهای جنریک با مدل اختیاری
    void LogTrace<TLogModel>(string message, TLogModel log = default);
    void LogDebug<TLogModel>(string message, TLogModel log = default);
    void LogInfo<TLogModel>(string message, TLogModel log = default);
    void LogWarning<TLogModel>(string message, TLogModel log = default);
    void LogError<TLogModel>(string message, TLogModel log = default, Exception exception = null);
    void LogCritical<TLogModel>(string message, TLogModel log = default, Exception exception = null);

    // متدهای Overload شده (فقط پیام/استثنا)
    void LogTrace(string message);
    void LogDebug(string message);
    void LogInfo(string message);
    void LogWarning(string message);
    void LogError(string message, Exception exception = null);
    void LogCritical(string message, Exception exception = null);
}