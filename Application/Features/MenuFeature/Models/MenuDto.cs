// Application/Features/Menu/Models/MenuDto.cs

namespace Application.Features.MenuFeature.Models;

public class MenuDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Route { get; set; }
    public int? ParentId { get; set; }
    public int Priority { get; set; }
    public bool IsActive { get; set; }

    /// <summary>
    /// زیرمنوها
    /// </summary>
    public List<MenuDto> SubMenus { get; set; } = [];
}