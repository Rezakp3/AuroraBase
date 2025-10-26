using System.Linq.Expressions;
using Application.Common.Models.Pagination;

namespace Infrastructure.Persistence.Helpers.Expressions;

/// <summary>
/// سازنده Expression Tree برای فیلترهای Cursor
/// </summary>
public static class ExpressionBuilder
{
    /// <summary>
    /// ساخت فیلتر ساده بر اساس Id
    /// </summary>
    public static BinaryExpression BuildIdFilter(
        MemberExpression idProperty,
        ConstantExpression lastIdConstant,
        SortOrder sortOrder) => sortOrder == SortOrder.Descending
            ? Expression.LessThan(idProperty, lastIdConstant)
            : Expression.GreaterThan(idProperty, lastIdConstant);

    /// <summary>
    /// ساخت فیلتر Keyset با دو فیلد (SortField + Id)
    /// فرمول: (SortField > lastValue) OR (SortField = lastValue AND Id > lastId)
    /// </summary>
    public static BinaryExpression BuildKeysetFilter(
        ParameterExpression parameter,
        string sortBy,
        object lastSortValue,
        Type sortPropertyType,
        MemberExpression idProperty,
        ConstantExpression lastIdConstant,
        SortOrder sortOrder)
    {
        var sortFieldProperty = Expression.Property(parameter, sortBy);
        var convertedLastSortValue = Convert.ChangeType(lastSortValue, sortPropertyType);
        var lastSortValueConstant = Expression.Constant(convertedLastSortValue, sortPropertyType);

        Expression sortFieldComparison;
        Expression sortFieldEquality;
        Expression idComparison;

        if (sortOrder == SortOrder.Descending)
        {
            sortFieldComparison = Expression.LessThan(sortFieldProperty, lastSortValueConstant);
            sortFieldEquality = Expression.Equal(sortFieldProperty, lastSortValueConstant);
            idComparison = Expression.LessThan(idProperty, lastIdConstant);
        }
        else
        {
            sortFieldComparison = Expression.GreaterThan(sortFieldProperty, lastSortValueConstant);
            sortFieldEquality = Expression.Equal(sortFieldProperty, lastSortValueConstant);
            idComparison = Expression.GreaterThan(idProperty, lastIdConstant);
        }

        var equalityAndIdCheck = Expression.AndAlso(sortFieldEquality, idComparison);
        return Expression.OrElse(sortFieldComparison, equalityAndIdCheck);
    }
}