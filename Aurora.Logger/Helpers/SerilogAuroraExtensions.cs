using Aurora.Logger.Models;
using Serilog.Context;
using System;
using System.Reflection;

namespace Aurora.Logger.Helpers;

internal static class SerilogAuroraExtensions
{
    // متد جنریک برای Push کردن Propertyهای هر مدل به LogContext (Req نهایی)
    internal static IDisposable PushProperties<TLogModel>(this TLogModel model)
    {
        var disposables = new List<IDisposable>();

        var properties = typeof(TLogModel).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            var value = prop.GetValue(model);

            // Push کردن Property به LogContext به صورت ساختاریافته (Destructure)
            var disposable = LogContext.PushProperty(prop.Name, value, destructureObjects: true);
            disposables.Add(disposable);
        }

        // یک Scope ترکیبی برای مدیریت تمام Propertyهای Push شده برمی‌گرداند
        return new DisposableScope(disposables);
    }

    // کلاس کمکی برای مدیریت Scopeهای متعدد (Disposables)
    private class DisposableScope(List<IDisposable> disposables) : IDisposable
    {
        public void Dispose()
        {
            foreach (var disposable in disposables)
            {
                disposable.Dispose();
            }
        }
    }
}