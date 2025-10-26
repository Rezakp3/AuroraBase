namespace Application.Common.Models.Pagination;

/// <summary>
/// گزینه‌های صفحه‌بندی برای درخواست‌های Page-Based با قابلیت Hybrid
/// </summary>
public class PagingOption
{
    private int _pageNumber = 1;
    private int _pageSize = 10;
    
    /// <summary>
    /// شماره صفحه (از 1 شروع می‌شود)
    /// </summary>
    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value < 1 ? 1 : value;
    }

    /// <summary>
    /// تعداد آیتم در هر صفحه (حداقل 1، حداکثر 100)
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

    // ✅ اضافه شده برای Hybrid Support
    
    /// <summary>
    /// ID آخرین آیتم از صفحه قبل (برای بهبود Performance)
    /// اگر مقداردهی شود، از Cursor-Based استفاده می‌کند به جای OFFSET
    /// </summary>
    public object? LastId { get; set; }

    /// <summary>
    /// مقدار فیلد مرتب‌سازی برای آخرین آیتم از صفحه قبل
    /// برای Keyset Pagination صحیح
    /// </summary>
    public object? LastSortValue { get; set; }

    /// <summary>
    /// آیا TotalCount محاسبه شود؟
    /// پیش‌فرض: true برای صفحه اول، false برای صفحات بعدی (بهبود Performance)
    /// </summary>
    public bool IncludeTotalCount { get; set; } = true;
}