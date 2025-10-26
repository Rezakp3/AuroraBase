namespace Application.Common.Models.Pagination;

/// <summary>
/// نتیجه صفحه‌بندی شده برای Cursor-Based Pagination
/// </summary>
public class CursorPaginatedList<T, TKey>(List<T> items, TKey? lastId, object? lastSortValue, int pageSize, bool hasMore) where TKey : struct
{
    public List<T> Items { get; set; } = items;

    /// <summary>
    /// ID آخرین آیتم در این بار دریافت (برای درخواست بعدی)
    /// </summary>
    public TKey? LastId { get; set; } = lastId;

    /// <summary>
    /// مقدار فیلد مرتب‌سازی برای آخرین آیتم (برای درخواست بعدی)
    /// فرانت باید این مقدار را در درخواست بعدی به عنوان LastSortValue ارسال کند
    /// </summary>
    public object? LastSortValue { get; set; } = lastSortValue;

    /// <summary>
    /// تعداد آیتم‌های درخواست شده
    /// </summary>
    public int PageSize { get; set; } = pageSize;

    /// <summary>
    /// آیا آیتم بیشتری وجود دارد؟
    /// </summary>
    public bool HasMore { get; set; } = hasMore;

    /// <summary>
    /// ایجاد یک نمونه جدید از CursorPaginatedList
    /// </summary>
    /// <param name="items">لیست آیتم‌ها</param>
    /// <param name="lastId">ID آخرین آیتم</param>
    /// <param name="lastSortValue">مقدار فیلد مرتب‌سازی برای آخرین آیتم</param>
    /// <param name="pageSize">تعداد آیتم در هر صفحه</param>
    /// <param name="hasMore">آیا آیتم بیشتری وجود دارد؟</param>
    /// <returns>نمونه جدید CursorPaginatedList</returns>
    public static CursorPaginatedList<T, TKey> Create(
        List<T> items,
        TKey? lastId,
        object? lastSortValue,
        int pageSize,
        bool hasMore) => new(items, lastId, lastSortValue, pageSize, hasMore);

    /// <summary>
    /// ایجاد یک CursorPaginatedList خالی
    /// </summary>
    /// <param name="pageSize">تعداد آیتم در هر صفحه</param>
    /// <returns>CursorPaginatedList خالی</returns>
    public static CursorPaginatedList<T, TKey> Empty(int pageSize = 10) 
        => new([], null, null, pageSize, false);
}