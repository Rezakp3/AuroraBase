using Aurora.Logger.Models;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System.Runtime.CompilerServices;
using Aurora.Logger.Helpers;

namespace Aurora.Logger.Services;


public class AuroraLogger(
    ILogger logger,
    AuroraLogOptions options) : IAuroraLogger
{

    // متد کمکی برای اجرای منطق اصلی لاگ (فقط یک بار نوشته می‌شود)
    private void LogInternal(
        LogLevel level,
        string message,
        Exception exception = null,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string memberName = "")
    {
        if (!options.IsEnabled || !logger.IsEnabled(level)) return;

        var className = Path.GetFileNameWithoutExtension(filePath);

        using var sourceScope = new SourceInfoScope(
            LogContext.PushProperty("SourceClass", className),
            LogContext.PushProperty("SourceLine", lineNumber),
            LogContext.PushProperty("SourceMember", memberName)
        );

        logger.Log(level, exception, "{Message}", message);

    }

    // متد کمکی برای اجرای منطق اصلی لاگ (فقط یک بار نوشته می‌شود)
    private void LogInternal<TLogModel>(
        LogLevel level,
        string message,
        TLogModel log = default,
        Exception exception = null,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string memberName = "")
    {
        if (!options.IsEnabled || !logger.IsEnabled(level)) return;

        var className = Path.GetFileNameWithoutExtension(filePath);

        using var sourceScope = new SourceInfoScope(
            LogContext.PushProperty("SourceClass", className),
            LogContext.PushProperty("SourceLine", lineNumber),
            LogContext.PushProperty("SourceMember", memberName)
        );

        // اگر مدل داده وجود داشته باشد، Propertyهای آن را اضافه کن
        using (log.PushProperties())
        {
            logger.Log(level, exception, "{Message}", message);
        }
    }

    // --- متدهای جنریک با مدل اختیاری ---
    public void LogTrace<TLogModel>(string message, TLogModel log = default)
        => LogInternal(LogLevel.Trace, message, log);

    public void LogDebug<TLogModel>(string message, TLogModel log = default)
        => LogInternal(LogLevel.Debug, message, log);

    public void LogInfo<TLogModel>(string message, TLogModel log = default)
        => LogInternal(LogLevel.Information, message, log);

    public void LogWarning<TLogModel>(string message, TLogModel log = default)
        => LogInternal(LogLevel.Warning, message, log);

    public void LogError<TLogModel>(string message, TLogModel log = default, Exception exception = null)
        => LogInternal(LogLevel.Error, message, log, exception);

    public void LogCritical<TLogModel>(string message, TLogModel log = default, Exception exception = null)
        => LogInternal(LogLevel.Critical, message, log, exception);

    // --- متدهای Overload شده (فقط پیام/استثنا) ---
    public void LogTrace(string message)
        => LogInternal(LogLevel.Trace, message);

    public void LogDebug(string message)
        => LogInternal(LogLevel.Debug, message);

    public void LogInfo(string message)
        => LogInternal(LogLevel.Information, message);

    public void LogWarning(string message)
        => LogInternal(LogLevel.Warning, message);

    public void LogError(string message, Exception exception = null)
        => LogInternal(LogLevel.Error, message, exception: exception);

    public void LogCritical(string message, Exception exception = null)
        => LogInternal(LogLevel.Critical, message, exception: exception);


}