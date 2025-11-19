using System.Linq.Expressions;
using Application.Common.Interfaces.Generals;
using Application.Common.Models.Pagination;
using Aurora.Cache.Services;

namespace Infrastructure.Persistence.Repositories.Base;

/// <summary>
/// کلاس پایه برای دکوریتورهای کش.
/// متدهای خواندن را کش می‌کند و متدهای نوشتن را مدیریت می‌کند.
/// </summary>
public abstract class CachedRepositoryBase<TEntity, TKey>(
    IRepository<TEntity, TKey> _innerRepo,
    ICacheService cache
    ) : IRepository<TEntity, TKey>
    where TEntity : class
    where TKey : struct
{
    // پیشوند کلید کش برای این موجودیت (مثلا "users")
    protected abstract string CachePrefix { get; }

    // زمان انقضای پیش‌فرض برای این موجودیت
    protected virtual TimeSpan DefaultExpiration => TimeSpan.FromMinutes(30);

    #region Cache Key Helpers

    protected virtual string GetIdKey(TKey id) => $"{CachePrefix}:{id}";
    protected virtual string GetAllKey() => $"{CachePrefix}:all";

    /// <summary>
    /// متدی که فرزندان باید پیاده‌سازی کنند تا بگویند با تغییر این موجودیت، چه کلیدهایی باید بسوزد؟
    /// </summary>
    protected abstract Task InvalidateRelatedCacheAsync(TEntity entity, CancellationToken ct);

    #endregion

    #region Query Methods (Cached)

    public virtual async Task<TEntity> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var key = GetIdKey(id);

        // استفاده از GetOrSet سرویس کش
        return await cache.GetOrSetAsync(
            key,
            () => _innerRepo.GetByIdAsync(id, cancellationToken),
            DefaultExpiration,
            cancellationToken
        );
    }

    public virtual async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // احتیاط: کش کردن GetAll فقط برای جداول کوچک (مثل تنظیمات یا شهرها) توصیه می‌شود
        // برای جداول بزرگ (مثل کاربران) این متد را در فرزند override کنید و کش را بردارید.
        var key = GetAllKey();

        return await cache.GetOrSetAsync(
            key,
            () => _innerRepo.GetAllAsync(cancellationToken),
            DefaultExpiration,
            cancellationToken
        );
    }

    // --- متدهای زیر به صورت پیش‌فرض کش نمی‌شوند (Pass-through) ---
    // دلیل: کش کردن کوئری‌های داینامیک (Predicate) بسیار پیچیده و پرهزینه است.

    public virtual Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => _innerRepo.FirstOrDefaultAsync(predicate, cancellationToken);

    public virtual Task<List<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => _innerRepo.GetWhereAsync(predicate, cancellationToken);

    public virtual Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => _innerRepo.AnyAsync(predicate, cancellationToken);

    public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default)
        => _innerRepo.CountAsync(predicate, cancellationToken);

    #endregion

    #region Paging Methods (Pass-through)

    // صفحه‌بندی را معمولاً کش نمی‌کنند مگر در صفحات اول (Landing Page)
    // اگر نیاز داشتید، در کلاس فرزند Override کنید.

    public virtual Task<PaginatedList<TEntity>> GetPagedAsync(PagingOption pagingOption, CancellationToken cancellationToken = default)
        => _innerRepo.GetPagedAsync(pagingOption, cancellationToken);

    public virtual Task<PaginatedList<TEntity>> GetPagedAsync(Expression<Func<TEntity, bool>> predicate, PagingOption pagingOption, CancellationToken cancellationToken = default)
        => _innerRepo.GetPagedAsync(predicate, pagingOption, cancellationToken);

    public virtual Task<CursorPaginatedList<TEntity, TKey>> GetCursorPagedAsync(CursorPagingOption<TKey> cursorPagingOption, CancellationToken cancellationToken = default)
        => _innerRepo.GetCursorPagedAsync(cursorPagingOption, cancellationToken);

    public virtual Task<CursorPaginatedList<TEntity, TKey>> GetCursorPagedAsync(Expression<Func<TEntity, bool>> predicate, CursorPagingOption<TKey> cursorPagingOption, CancellationToken cancellationToken = default)
        => _innerRepo.GetCursorPagedAsync(predicate, cursorPagingOption, cancellationToken);

    #endregion

    #region Command Methods (Write-Through / Invalidation)

    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var result = await _innerRepo.AddAsync(entity, cancellationToken);
        // بعد از اضافه کردن، باید کش‌های لیست (GetAll) یا مرتبط را پاک کنیم
        await InvalidateRelatedCacheAsync(result, cancellationToken);
        return result;
    }

    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await _innerRepo.AddRangeAsync(entities, cancellationToken);
        foreach (var entity in entities)
        {
            await InvalidateRelatedCacheAsync(entity, cancellationToken);
        }
    }

    public virtual void Update(TEntity entity)
    {
        // نکته: چون متد void است، نمی‌توانیم اینجا await کنیم. 
        // در معماری مدرن بهتر است UpdateAsync داشته باشید.
        // اما طبق اینترفیس شما:
        _innerRepo.Update(entity);

        // اینجا مجبوریم Fire-and-Forget اجرا کنیم یا بلاکینگ (که بد است).
        // راه حل درست: تغییر اینترفیس به Task UpdateAsync.
        // راه حل موقت:
        Task.Run(() => InvalidateRelatedCacheAsync(entity, CancellationToken.None));
    }

    public virtual void UpdateRange(IEnumerable<TEntity> entities)
    {
        _innerRepo.UpdateRange(entities);
        Task.Run(async () =>
        {
            foreach (var e in entities) await InvalidateRelatedCacheAsync(e, CancellationToken.None);
        });
    }

    public virtual void Delete(TEntity entity)
    {
        _innerRepo.Delete(entity);
        Task.Run(() => InvalidateRelatedCacheAsync(entity, CancellationToken.None));
    }

    public virtual void DeleteRange(IEnumerable<TEntity> entities)
    {
        _innerRepo.DeleteRange(entities);
        Task.Run(async () =>
        {
            foreach (var e in entities) await InvalidateRelatedCacheAsync(e, CancellationToken.None);
        });
    }

    public virtual async Task<bool> DeleteByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        // اول موجودیت را می‌گیریم تا بتوانیم کش‌های مرتبطش را پیدا کنیم
        var entity = await _innerRepo.GetByIdAsync(id, cancellationToken);
        if (entity == null) return false;

        var result = await _innerRepo.DeleteByIdAsync(id, cancellationToken);
        if (result)
        {
            await InvalidateRelatedCacheAsync(entity, cancellationToken);
            // خود کلید ID را هم پاک کن
            await cache.RemoveAsync(GetIdKey(id), cancellationToken);
        }
        return result;
    }

    #endregion
}