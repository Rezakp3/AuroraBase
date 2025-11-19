using System.Linq.Expressions;
using Application.Common.Models.Pagination;
using Infrastructure.Persistence.Helpers.Expressions;

namespace Infrastructure.Persistence.Helpers.Cursor;

public static class CursorFilterBuilder
{
    public static IQueryable<TEntity> ApplyCursorFilter<TEntity>(
        this IQueryable<TEntity> query,
        object lastId,
        object lastSortValue,
        string sortBy,
        System.Reflection.PropertyInfo sortProperty,
        SortOrder sortOrder,
        bool isSortingById) where TEntity : class
    {
        var parameter = Expression.Parameter(typeof(TEntity), "x");
        var idProperty = Expression.Property(parameter, "Id");
        
        // تبدیل lastId به نوع صحیح
        var idPropertyType = idProperty.Type;
        var convertedLastId = Convert.ChangeType(lastId, idPropertyType);
        var lastIdConstant = Expression.Constant(convertedLastId, idPropertyType);

        Expression filterExpression;

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

        var lambda = Expression.Lambda<Func<TEntity, bool>>(filterExpression, parameter);
        return query.Where(lambda);
    }
}