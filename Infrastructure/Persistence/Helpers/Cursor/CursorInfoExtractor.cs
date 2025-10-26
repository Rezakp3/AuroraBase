namespace Infrastructure.Persistence.Helpers.Cursor;

/// <summary>
/// استخراج‌کننده اطلاعات Cursor از آیتم‌ها
/// </summary>
public static class CursorInfoExtractor
{
    /// <summary>
    /// استخراج LastId و LastSortValue از آخرین آیتم
    /// </summary>
    public static (object? lastId, object? lastSortValue) ExtractCursorInfo<TEntity>(
        List<TEntity> items,
        string sortBy,
        bool isSortingById) where TEntity : class
    {
        if (items.Count == 0)
            return (null, null);

        var lastItem = items.Last();

        var idProperty = typeof(TEntity).GetProperty("Id");
        var lastId = idProperty?.GetValue(lastItem);

        object? lastSortValue = null;
        if (!isSortingById)
        {
            var sortProperty = typeof(TEntity).GetProperty(sortBy);
            lastSortValue = sortProperty?.GetValue(lastItem);
        }

        return (lastId, lastSortValue);
    }

    /// <summary>
    /// استخراج LastId و LastSortValue با Generic Type
    /// </summary>
    public static (TKey? lastId, object? lastSortValue) ExtractCursorInfo<TEntity, TKey>(
        List<TEntity> items,
        string sortBy,
        bool isSortingById) 
        where TEntity : class
        where TKey : struct
    {
        if (items.Count == 0)
            return (null, null);

        var lastItem = items.Last();

        var idProperty = typeof(TEntity).GetProperty("Id");
        var lastId = idProperty != null ? (TKey?)idProperty.GetValue(lastItem) : null;

        object? lastSortValue = null;
        if (!isSortingById)
        {
            var sortProperty = typeof(TEntity).GetProperty(sortBy);
            lastSortValue = sortProperty?.GetValue(lastItem);
        }

        return (lastId, lastSortValue);
    }
}