namespace Application.Common.Models.Pagination;

/// <summary>
/// نتیجه صفحه‌بندی شده برای Page-Based Pagination با قابلیت Hybrid
/// </summary>
public class PaginatedList<T>(
    List<T> items,
    int pageNumber,
    int pageSize,
    int? totalCount = null,
    object? lastId = null,
    object? lastSortValue = null,
    bool? hasMore = null)
{
    public List<T> Items { get; set; } = items;

    // ✅ Page-Based Properties
    public int PageNumber { get; set; } = pageNumber;
    public int PageSize { get; set; } = pageSize;
    public int? TotalCount { get; set; } = totalCount;
    public int? TotalPages => TotalCount.HasValue 
        ? (int)Math.Ceiling(TotalCount.Value / (double)PageSize) 
        : null;
    public bool HasPreviousPage => PageNumber > 1;
    public bool? HasNextPage => TotalCount.HasValue 
        ? PageNumber < TotalPages 
        : HasMore;  // اگر TotalCount نداریم، از HasMore استفاده می‌کنیم
    public int? PreviousPageNumber => HasPreviousPage ? PageNumber - 1 : null;
    public int? NextPageNumber => HasNextPage == true ? PageNumber + 1 : null;

    // ✅ Cursor-Based Properties (اضافه شده)

    /// <summary>
    /// ID آخرین آیتم در این صفحه (برای درخواست صفحه بعدی)
    /// فرانت باید این مقدار را در درخواست بعدی به عنوان LastId ارسال کند
    /// </summary>
    public object? LastId { get; set; } = lastId;

    /// <summary>
    /// مقدار فیلد مرتب‌سازی برای آخرین آیتم (برای درخواست صفحه بعدی)
    /// </summary>
    public object? LastSortValue { get; set; } = lastSortValue;

    /// <summary>
    /// آیا صفحه بعدی وجود دارد؟ (زمانی که TotalCount محاسبه نشده)
    /// </summary>
    public bool? HasMore { get; set; } = hasMore;

    public static PaginatedList<T> Create(
        List<T> items,
        int pageNumber,
        int pageSize,
        int? totalCount = null,
        object? lastId = null,
        object? lastSortValue = null,
        bool? hasMore = null) 
        => new(items, pageNumber, pageSize, totalCount, lastId, lastSortValue, hasMore);

    public static PaginatedList<T> Empty(int pageNumber = 1, int pageSize = 10) 
        => new([], pageNumber, pageSize, 0, null, null, false);
}