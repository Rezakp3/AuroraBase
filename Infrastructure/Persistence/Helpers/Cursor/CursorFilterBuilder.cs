using System.Linq.Expressions;
using Application.Common.Models.Pagination;
using Infrastructure.Persistence.Helpers.Expressions;

namespace Infrastructure.Persistence.Helpers.Cursor;

/// <summary>
/// سازنده فیلترهای Cursor برای Pagination
/// </summary>
public static class CursorFilterBuilder
{
    /// <summary>
    /// اعمال فیلتر Cursor بصورت Dynamic
    /// </summary>
    public static IQueryable<TEntity> ApplyCursorFilter<TEntity>(
        this IQueryable<TEntity> query,
        object lastId,
        object? lastSortValue,
        string sortBy,
        System.Reflection.PropertyInfo? sortProperty,
        SortOrder sortOrder,
        bool isSortingById) where TEntity : class
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(TEntity), "x");
        var idProperty = System.Linq.Expressions.Expression.Property(parameter, "Id");
        
        // تبدیل lastId به نوع صحیح
        var idPropertyType = idProperty.Type;
        var convertedLastId = Convert.ChangeType(lastId, idPropertyType);
        var lastIdConstant = System.Linq.Expressions.Expression.Constant(convertedLastId, idPropertyType);

        System.Linq.Expressions.Expression filterExpression;

        if (isSortingById)
        {
            // فیلتر ساده بر اساس Id
            filterExpression = ExpressionBuilder.BuildIdFilter(idProperty, lastIdConstant, sortOrder);
        }
        else if (lastSortValue != null && sortProperty != null)
        {
            // Keyset Pagination با دو فیلد
            filterExpression = ExpressionBuilder.BuildKeysetFilter(
                parameter,
                sortBy,
                lastSortValue,
                sortProperty.PropertyType,
                idProperty,
                lastIdConstant,
                sortOrder);
        }
        else
        {
            // Fallback به Id
            filterExpression = ExpressionBuilder.BuildIdFilter(idProperty, lastIdConstant, sortOrder);
        }

        var lambda = System.Linq.Expressions.Expression.Lambda<Func<TEntity, bool>>(filterExpression, parameter);
        return query.Where(lambda);
    }
}