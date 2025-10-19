namespace Core.Entities.Auth;

public class Service : BaseEntity<int>
{
    #region Properties

    public string ServiceName { get; set; }
    public string Description { get; set; }
    public string Address { get; set; }

    #endregion
    #region relations

    public ICollection<Menu> Menus { get; set; }
    public ICollection<Role> Roles { get; set; }

    #endregion
}

