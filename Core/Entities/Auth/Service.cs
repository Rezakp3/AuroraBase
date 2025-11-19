using Core.Entities.Auth.Relation;

namespace Core.Entities.Auth;

public class Service : BaseEntity<int>
{
    #region Properties

    public string ServiceName { get; set; }
    public string Description { get; set; }
    public string Address { get; set; }
    public string ServiceIdentifier { get; set; }
    #endregion

    #region relations

    public ICollection<MenuService> MenuServices { get; set; } = [];
    public ICollection<RoleService> RoleServices { get; set; } = [];

    #endregion
}

