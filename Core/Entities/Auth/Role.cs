namespace Core.Entities.Auth;

public class Role : BaseEntity<int>
{
    #region Properties

    public string Name { get; set; }
    public string Title { get; set; }

    #endregion


    #region relation

    public ICollection<Menu> Menus { get; set; } = [];
    public ICollection<Service> Services { get; set; } = [];
    public ICollection<RoleClaim> Claims { get; set; } = [];
    public ICollection<User> Users { get; set; } = [];

    #endregion
}
