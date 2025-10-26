namespace Core.Entities.Auth;

public class UserClaim : BaseEntity<int>
{
    #region Properties

    public string ClaimType { get; set; }
    public string ClaimValue { get; set; }

    #endregion

    #region Relation

    public long UserId { get; set; }
    public User User { get; set; }

    #endregion
}
