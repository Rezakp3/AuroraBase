namespace Core.Entities.Auth;

public class RoleClaim : BaseEntity<int>
{
    #region Properties

    public string Type { get; set; }
    public string Value { get; set; }

    #endregion

    #region relation
    public int RoleId { get; set; }
    public Role Role { get; set; } 
    #endregion
}
