namespace Aurora.Logger.Models;

// این کلاس برای مدیریت همزمان چند IDisposable لازم است
internal class SourceInfoScope(IDisposable classScope, IDisposable lineScope, IDisposable memberScope) : IDisposable
{
    public void Dispose()
    {
        memberScope.Dispose();
        lineScope.Dispose();
        classScope.Dispose();
    }
}