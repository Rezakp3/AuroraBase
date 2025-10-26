using Core.Entities.Auth.Relation;
using Core.Interfaces;

namespace Core.Entities.Auth;

public class Role : BaseEntity<int>
{
    #region Properties

    public string Name { get; set; }
    public string Title { get; set; }

    #endregion

    #region relation

    public ICollection<RoleMenu> RoleMenus { get; set; } = [];
    public ICollection<RoleService> RoleServices { get; set; } = [];
    public ICollection<UserRole> UserRoles { get; set; } = [];
    public ICollection<RoleClaim> Claims { get; set; } = [];

    #endregion
}
