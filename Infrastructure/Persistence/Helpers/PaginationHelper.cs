using Application.Common.Models.Pagination;
using Infrastructure.Persistence.Helpers.Cursor;
using Infrastructure.Persistence.Helpers.Sorting;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Helpers;

/// <summary>
/// کلاس کمکی اصلی برای عملیات صفحه‌بندی
/// </summary>
public static class PaginationHelper
{
    #region Page-Based Pagination (Hybrid)

    /// <summary>
    /// اعمال صفحه‌بندی Page-Based با قابلیت Hybrid
    /// </summary>
    public static async Task<PaginatedList<TEntity>> ApplyPageBasedPaginationAsync<TEntity>(
        IQueryable<TEntity> query,
        PagingOption pagingOption,
        CancellationToken cancellationToken) where TEntity : class
    {
        // اعتبارسنجی و مرتب‌سازی
        var (sortBy, sortProperty) = QuerySortingExtensions.ValidateAndGetSortProperty<TEntity>(pagingOption.SortBy);
        var isSortingById = sortBy.Equals("Id", StringComparison.OrdinalIgnoreCase);
        
        query = query.ApplySortingWithSecondaryId(pagingOption.SortBy, pagingOption.SortOrder);

        // شمارش کل (اختیاری)
        int? totalCount = await GetTotalCountIfRequested(query, pagingOption.IncludeTotalCount, cancellationToken);
        
        if (totalCount == 0)
            return PaginatedList<TEntity>.Empty(pagingOption.PageNumber, pagingOption.PageSize);

        // اعمال فیلتر (Cursor یا Offset)
        query = ApplyPagingFilter(query, pagingOption, sortBy, sortProperty, isSortingById);

        // دریافت آیتم‌ها
        var (items, hasMore) = await FetchItemsWithHasMore(query, pagingOption.PageSize, cancellationToken);

        // استخراج Cursor Info
        var (lastId, lastSortValue) = CursorInfoExtractor.ExtractCursorInfo(items, sortBy, isSortingById);

        return PaginatedList<TEntity>.Create(
            items, pagingOption.PageNumber, pagingOption.PageSize,
            totalCount, lastId, lastSortValue, hasMore);
    }

    /// <summary>
    /// شمارش کل آیتم‌ها (اگر درخواست شده باشد)
    /// </summary>
    private static async Task<int?> GetTotalCountIfRequested<TEntity>(
        IQueryable<TEntity> query,
        bool includeTotalCount,
        CancellationToken cancellationToken) where TEntity : class
    {
        return includeTotalCount ? await query.CountAsync(cancellationToken) : null;
    }

    /// <summary>
    /// اعمال فیلتر Paging (Cursor یا Offset)
    /// </summary>
    private static IQueryable<TEntity> ApplyPagingFilter<TEntity>(
        IQueryable<TEntity> query,
        PagingOption pagingOption,
        string sortBy,
        System.Reflection.PropertyInfo sortProperty,
        bool isSortingById) where TEntity : class
    {
        bool canUseCursor = pagingOption.LastId != null && 
                            (isSortingById || pagingOption.LastSortValue != null);

        if (canUseCursor)
        {
            // استفاده از Cursor (سریع)
            return query.ApplyCursorFilter(
                pagingOption.LastId!,
                pagingOption.LastSortValue,
                sortBy,
                sortProperty,
                pagingOption.SortOrder,
                isSortingById);
        }
        else if (pagingOption.PageNumber > 1)
        {
            // Fallback به Offset
            return query.Skip((pagingOption.PageNumber - 1) * pagingOption.PageSize);
        }

        return query;
    }

    /// <summary>
    /// دریافت آیتم‌ها با تشخیص HasMore
    /// </summary>
    private static async Task<(List<TEntity> items, bool hasMore)> FetchItemsWithHasMore<TEntity>(
        IQueryable<TEntity> query,
        int pageSize,
        CancellationToken cancellationToken) where TEntity : class
    {
        var items = await query.Take(pageSize + 1).ToListAsync(cancellationToken);
        var hasMore = items.Count > pageSize;
        
        if (hasMore)
            items.RemoveAt(items.Count - 1);

        return (items, hasMore);
    }

    #endregion

    #region Cursor-Based Pagination

    /// <summary>
    /// اعمال صفحه‌بندی Cursor-Based
    /// </summary>
    public static async Task<CursorPaginatedList<TEntity, TKey>> ApplyCursorBasedPaginationAsync<TEntity, TKey>(
        IQueryable<TEntity> query,
        CursorPagingOption<TKey> cursorPagingOption,
        CancellationToken cancellationToken)
        where TEntity : class
        where TKey : struct
    {
        // اعتبارسنجی و مرتب‌سازی
        var (sortBy, sortProperty) = QuerySortingExtensions.ValidateAndGetSortProperty<TEntity>(cursorPagingOption.SortBy);
        var isSortingById = sortBy.Equals("Id", StringComparison.OrdinalIgnoreCase);

        query = query.ApplySortingWithSecondaryId(cursorPagingOption.SortBy, cursorPagingOption.SortOrder);

        // اعمال فیلتر Cursor (اگر LastId موجود باشد)
        if (cursorPagingOption.LastId.HasValue)
        {
            query = query.ApplyCursorFilter(
                cursorPagingOption.LastId.Value,
                cursorPagingOption.LastSortValue,
                sortBy,
                sortProperty,
                cursorPagingOption.SortOrder,
                isSortingById);
        }

        // دریافت آیتم‌ها
        var (items, hasMore) = await FetchItemsWithHasMore(query, cursorPagingOption.PageSize, cancellationToken);

        // استخراج Cursor Info
        var (lastId, lastSortValue) = CursorInfoExtractor.ExtractCursorInfo<TEntity, TKey>(items, sortBy, isSortingById);

        return CursorPaginatedList<TEntity, TKey>.Create(
            items, lastId, lastSortValue, cursorPagingOption.PageSize, hasMore);
    }

    #endregion
}