using System.Linq.Expressions;
using Application.Common.Interfaces.Generals;
using Application.Common.Models.Pagination;
using Infrastructure.Persistence.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// پیاده‌سازی پایه Repository برای عملیات CRUD و صفحه‌بندی
/// </summary>
/// <typeparam name="TEntity">نوع موجودیت</typeparam>
/// <typeparam name="TKey">نوع کلید اصلی</typeparam>
public class Repository<TEntity, TKey> : IRepository<TEntity, TKey>
    where TEntity : class
    where TKey : struct
{
    protected readonly MyContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public Repository(MyContext context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
    }

    #region Query Methods

    /// <summary>
    /// دریافت یک موجودیت بر اساس شناسه
    /// </summary>
    public virtual async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
        => await _dbSet.FindAsync([id], cancellationToken);

    /// <summary>
    /// دریافت اولین موجودیت که شرط را برآورده می‌کند
    /// </summary>
    public virtual async Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);

    /// <summary>
    /// دریافت تمام موجودیت‌ها
    /// </summary>
    public virtual async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbSet.ToListAsync(cancellationToken);

    /// <summary>
    /// دریافت موجودیت‌هایی که شرط را برآورده می‌کنند
    /// </summary>
    public virtual async Task<List<TEntity>> GetWhereAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        => await _dbSet.Where(predicate).ToListAsync(cancellationToken);

    /// <summary>
    /// بررسی وجود حداقل یک موجودیت که شرط را برآورده کند
    /// </summary>
    public virtual async Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        => await _dbSet.AnyAsync(predicate, cancellationToken);

    /// <summary>
    /// شمارش تعداد موجودیت‌ها
    /// </summary>
    public virtual async Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
        => predicate == null
            ? await _dbSet.CountAsync(cancellationToken)
            : await _dbSet.CountAsync(predicate, cancellationToken);

    #endregion

    #region Paging Methods

    /// <summary>
    /// دریافت لیست صفحه‌بندی شده با شماره صفحه (Page-Based Pagination)
    /// </summary>
    public virtual async Task<PaginatedList<TEntity>> GetPagedAsync(
        PagingOption pagingOption,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();
        return await PaginationHelper.ApplyPageBasedPaginationAsync(query, pagingOption, cancellationToken);
    }

    /// <summary>
    /// دریافت لیست صفحه‌بندی شده با فیلتر و شماره صفحه (Page-Based Pagination)
    /// </summary>
    public virtual async Task<PaginatedList<TEntity>> GetPagedAsync(
        Expression<Func<TEntity, bool>> predicate,
        PagingOption pagingOption,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(predicate);
        return await PaginationHelper.ApplyPageBasedPaginationAsync(query, pagingOption, cancellationToken);
    }

    /// <summary>
    /// دریافت لیست صفحه‌بندی شده بر اساس Cursor (Cursor-Based Pagination)
    /// </summary>
    public virtual async Task<CursorPaginatedList<TEntity, TKey>> GetCursorPagedAsync(
        CursorPagingOption<TKey> cursorPagingOption,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();
        return await PaginationHelper.ApplyCursorBasedPaginationAsync<TEntity, TKey>(query, cursorPagingOption, cancellationToken);
    }

    /// <summary>
    /// دریافت لیست صفحه‌بندی شده با فیلتر بر اساس Cursor (Cursor-Based Pagination)
    /// </summary>
    public virtual async Task<CursorPaginatedList<TEntity, TKey>> GetCursorPagedAsync(
        Expression<Func<TEntity, bool>> predicate,
        CursorPagingOption<TKey> cursorPagingOption,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(predicate);
        return await PaginationHelper.ApplyCursorBasedPaginationAsync<TEntity, TKey>(query, cursorPagingOption, cancellationToken);
    }

    #endregion

    #region Command Methods

    /// <summary>
    /// افزودن یک موجودیت جدید
    /// </summary>
    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var entry = await _dbSet.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    /// <summary>
    /// افزودن چندین موجودیت به صورت یکجا
    /// </summary>
    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        => await _dbSet.AddRangeAsync(entities, cancellationToken);

    /// <summary>
    /// بروزرسانی یک موجودیت موجود
    /// </summary>
    public virtual void Update(TEntity entity)
        => _dbSet.Update(entity);

    /// <summary>
    /// بروزرسانی چندین موجودیت به صورت یکجا
    /// </summary>
    public virtual void UpdateRange(IEnumerable<TEntity> entities)
        => _dbSet.UpdateRange(entities);

    /// <summary>
    /// حذف یک موجودیت
    /// </summary>
    public virtual void Delete(TEntity entity)
        => _dbSet.Remove(entity);

    /// <summary>
    /// حذف چندین موجودیت به صورت یکجا
    /// </summary>
    public virtual void DeleteRange(IEnumerable<TEntity> entities)
        => _dbSet.RemoveRange(entities);

    /// <summary>
    /// حذف یک موجودیت بر اساس شناسه
    /// </summary>
    public virtual async Task<bool> DeleteByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return false;

        Delete(entity);
        return true;
    }

    #endregion
}