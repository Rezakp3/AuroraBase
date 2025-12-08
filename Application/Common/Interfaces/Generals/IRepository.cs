using System.Linq.Expressions;
using Application.Common.Models.Pagination;

namespace Application.Common.Interfaces.Generals;

/// <summary>
/// رابط پایه Repository برای عملیات CRUD و صفحه‌بندی
/// </summary>
/// <typeparam name="TEntity">نوع موجودیت</typeparam>
/// <typeparam name="TKey">نوع کلید اصلی</typeparam>
public interface IRepository<TEntity, TKey> where TEntity : class where TKey : struct
{
    #region Query Methods

    /// <summary>
    /// دریافت یک موجودیت بر اساس شناسه
    /// </summary>
    /// <param name="id">شناسه موجودیت</param>
    /// <param name="cancellationToken">توکن لغو عملیات</param>
    /// <returns>موجودیت یافت شده یا null</returns>
    Task<TEntity> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت اولین موجودیت که شرط را برآورده می‌کند
    /// </summary>
    /// <param name="predicate">شرط جستجو</param>
    /// <param name="cancellationToken">توکن لغو عملیات</param>
    /// <returns>موجودیت یافت شده یا null</returns>
    Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت تمام موجودیت‌ها
    /// </summary>
    /// <param name="cancellationToken">توکن لغو عملیات</param>
    /// <returns>لیست تمام موجودیت‌ها</returns>
    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت موجودیت‌هایی که شرط را برآورده می‌کنند
    /// </summary>
    /// <param name="predicate">شرط جستجو</param>
    /// <param name="cancellationToken">توکن لغو عملیات</param>
    /// <returns>لیست موجودیت‌های یافت شده</returns>
    Task<List<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// بررسی وجود حداقل یک موجودیت که شرط را برآورده کند
    /// </summary>
    /// <param name="predicate">شرط جستجو</param>
    /// <param name="cancellationToken">توکن لغو عملیات</param>
    /// <returns>true اگر موجودیت وجود داشته باشد، در غیر این صورت false</returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// شمارش تعداد موجودیت‌ها
    /// </summary>
    /// <param name="predicate">شرط جستجو (اختیاری)</param>
    /// <param name="cancellationToken">توکن لغو عملیات</param>
    /// <returns>تعداد موجودیت‌ها</returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);

    #endregion

    #region Paging Methods

    /// <summary>
    /// دریافت لیست صفحه‌بندی شده با شماره صفحه
    /// </summary>
    /// <param name="pagingOption">گزینه‌های صفحه‌بندی شامل شماره صفحه و تعداد آیتم</param>
    /// <param name="cancellationToken">توکن لغو عملیات</param>
    /// <returns>لیست صفحه‌بندی شده شامل آیتم‌ها و اطلاعات صفحه‌بندی</returns>
    Task<PaginatedList<TEntity>> GetPagedAsync(
        PagingOption pagingOption,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت لیست صفحه‌بندی شده با فیلتر و شماره صفحه
    /// </summary>
    /// <param name="predicate">شرط فیلتر</param>
    /// <param name="pagingOption">گزینه‌های صفحه‌بندی شامل شماره صفحه و تعداد آیتم</param>
    /// <param name="cancellationToken">توکن لغو عملیات</param>
    /// <returns>لیست صفحه‌بندی شده فیلتر شده شامل آیتم‌ها و اطلاعات صفحه‌بندی</returns>
    Task<PaginatedList<TEntity>> GetPagedAsync(
        Expression<Func<TEntity, bool>> predicate,
        PagingOption pagingOption,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت لیست صفحه‌بندی شده بر اساس ID آخرین آیتم (Cursor-Based Pagination)
    /// </summary>
    /// <param name="cursorPagingOption">گزینه‌های صفحه‌بندی شامل ID آخرین آیتم و تعداد</param>
    /// <param name="cancellationToken">توکن لغو عملیات</param>
    /// <returns>لیست صفحه‌بندی شده شامل آیتم‌ها و ID آخرین آیتم</returns>
    Task<CursorPaginatedList<TEntity, TKey>> GetCursorPagedAsync(
        CursorPagingOption<TKey> cursorPagingOption,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت لیست صفحه‌بندی شده با فیلتر بر اساس ID آخرین آیتم (Cursor-Based Pagination)
    /// </summary>
    /// <param name="predicate">شرط فیلتر</param>
    /// <param name="cursorPagingOption">گزینه‌های صفحه‌بندی شامل ID آخرین آیتم و تعداد</param>
    /// <param name="cancellationToken">توکن لغو عملیات</param>
    /// <returns>لیست صفحه‌بندی شده فیلتر شده شامل آیتم‌ها و ID آخرین آیتم</returns>
    Task<CursorPaginatedList<TEntity, TKey>> GetCursorPagedAsync(
        Expression<Func<TEntity, bool>> predicate,
        CursorPagingOption<TKey> cursorPagingOption,
        CancellationToken cancellationToken = default);

    #endregion

    #region Command Methods

    /// <summary>
    /// افزودن یک موجودیت جدید
    /// </summary>
    /// <param name="entity">موجودیت برای افزودن</param>
    /// <param name="cancellationToken">توکن لغو عملیات</param>
    /// <returns>موجودیت افزوده شده</returns>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// افزودن چندین موجودیت به صورت یکجا
    /// </summary>
    /// <param name="entities">لیست موجودیت‌ها برای افزودن</param>
    /// <param name="cancellationToken">توکن لغو عملیات</param>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// بروزرسانی یک موجودیت موجود
    /// </summary>
    /// <param name="entity">موجودیت برای بروزرسانی</param>
    void Update(TEntity entity);

    /// <summary>
    /// بروزرسانی چندین موجودیت به صورت یکجا
    /// </summary>
    /// <param name="entities">لیست موجودیت‌ها برای بروزرسانی</param>
    void UpdateRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// حذف یک موجودیت
    /// </summary>
    /// <param name="entity">موجودیت برای حذف</param>
    void Delete(TEntity entity);

    /// <summary>
    /// حذف چندین موجودیت به صورت یکجا
    /// </summary>
    /// <param name="entities">لیست موجودیت‌ها برای حذف</param>
    void DeleteRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// حذف یک موجودیت بر اساس شناسه
    /// </summary>
    /// <param name="id">شناسه موجودیت</param>
    /// <param name="cancellationToken">توکن لغو عملیات</param>
    /// <returns>true اگر موجودیت حذف شد، false اگر موجودیت یافت نشد</returns>
    Task<bool> DeleteByIdAsync(TKey id, CancellationToken cancellationToken = default);

    #endregion
}