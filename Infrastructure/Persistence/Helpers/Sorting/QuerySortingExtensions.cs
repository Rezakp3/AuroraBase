using System.Linq.Expressions;
using Application.Common.Models.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Helpers.Sorting;

/// <summary>
/// Extension Methods برای مرتب‌سازی Query
/// </summary>
public static class QuerySortingExtensions
{
    /// <summary>
    /// اعمال مرتب‌سازی بر اساس PagingOption
    /// </summary>
    public static IQueryable<TEntity> ApplySorting<TEntity>(
        this IQueryable<TEntity> query,
        string? sortBy,
        SortOrder sortOrder) where TEntity : class
    {
        var validSortBy = ValidateSortField<TEntity>(sortBy);

        return sortOrder == SortOrder.Descending
            ? query.OrderByDescending(e => EF.Property<object>(e, validSortBy))
            : query.OrderBy(e => EF.Property<object>(e, validSortBy));
    }

    /// <summary>
    /// اعمال مرتب‌سازی با ترتیب ثانویه بر اساس Id
    /// </summary>
    public static IQueryable<TEntity> ApplySortingWithSecondaryId<TEntity>(
        this IQueryable<TEntity> query,
        string? sortBy,
        SortOrder sortOrder) where TEntity : class
    {
        var validSortBy = ValidateSortField<TEntity>(sortBy);
        var isSortingById = validSortBy.Equals("Id", StringComparison.OrdinalIgnoreCase);

        // مرتب‌سازی اولیه
        query = sortOrder == SortOrder.Descending
            ? query.OrderByDescending(e => EF.Property<object>(e, validSortBy))
            : query.OrderBy(e => EF.Property<object>(e, validSortBy));

        // اگر مرتب‌سازی بر اساس Id نیست، Id را به عنوان ثانویه اضافه کن
        if (!isSortingById)
        {
            query = sortOrder == SortOrder.Descending
                ? ((IOrderedQueryable<TEntity>)query).ThenByDescending(e => EF.Property<object>(e, "Id"))
                : ((IOrderedQueryable<TEntity>)query).ThenBy(e => EF.Property<object>(e, "Id"));
        }

        return query;
    }

    /// <summary>
    /// اعتبارسنجی فیلد مرتب‌سازی
    /// </summary>
    public static string ValidateSortField<TEntity>(string? sortBy)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return "Id";

        var propertyInfo = typeof(TEntity).GetProperty(sortBy);
        return propertyInfo == null ? "Id" : sortBy;
    }

    /// <summary>
    /// اعتبارسنجی و دریافت PropertyInfo
    /// </summary>
    public static (string validSortBy, System.Reflection.PropertyInfo? propertyInfo) ValidateAndGetSortProperty<TEntity>(string? sortBy)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            sortBy = "Id";

        var propertyInfo = typeof(TEntity).GetProperty(sortBy);
        if (propertyInfo == null)
        {
            sortBy = "Id";
            propertyInfo = typeof(TEntity).GetProperty(sortBy);
        }

        return (sortBy, propertyInfo);
    }
}