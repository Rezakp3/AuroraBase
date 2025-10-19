using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Auth;

public class Menu : BaseEntity<int>
{
    #region properties

    public string Title { get; set; }
    public string Route { get; set; }
    public int? ParentId { get; set; }
    public Menu? Parent { get; set; }

    #endregion

    #region relation

    public ICollection<Menu> SubMenu { get; set; } = [];
    public ICollection<Role> Roles { get; set; } = [];
    public ICollection<Service> Services { get; set; } = [];

    #endregion
}
