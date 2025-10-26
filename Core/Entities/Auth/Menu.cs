using System.ComponentModel.DataAnnotations;
using Core.Entities.Auth.Relation;

namespace Core.Entities.Auth;

public class Menu : BaseEntity<int>
{
    #region properties

    public string Title { get; set; }
    public string Route { get; set; }
    public int? ParentId { get; set; }

    #endregion

    #region relation

    public Menu? Parent { get; set; }
    public ICollection<Menu> SubMenu { get; set; } = [];
    public ICollection<RoleMenu> RoleMenus { get; set; } = [];
    public ICollection<MenuService> MenuServices { get; set; } = [];

    #endregion
}
