namespace Application.Common.Models.Pagination;

/// <summary>
/// گزینه‌های صفحه‌بندی برای درخواست‌های Cursor-Based (بر اساس ID آخرین آیتم)
/// </summary>
public class CursorPagingOption<TKey> where TKey : struct
{
    private int _pageSize = 10;

    /// <summary>
    /// ID آخرین آیتمی که فرانت دریافت کرده است (null = از اول شروع کن)
    /// </summary>
    public TKey? LastId { get; set; }

    /// <summary>
    /// مقدار فیلد مرتب‌سازی برای آخرین آیتم (برای Keyset Pagination صحیح)
    /// مثال: اگر SortBy = "CreatedDate" باشد، این مقدار باید آخرین CreatedDate دریافت شده باشد
    /// </summary>
    public object? LastSortValue { get; set; }

    /// <summary>
    /// تعداد آیتم‌های مورد نیاز (حداقل 1، حداکثر 100)
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value switch
        {
            < 1 => 10,
            > 100 => 100,
            _ => value
        };
    }

    /// <summary>
    /// نام فیلد برای مرتب‌سازی
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// جهت مرتب‌سازی
    /// </summary>
    public SortOrder SortOrder { get; set; } = SortOrder.Ascending;
}