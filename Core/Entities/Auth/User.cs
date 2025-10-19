using Core.Enums;

namespace Core.Entities.Auth;

public class User : BaseEntityWithDate<long>
{
    #region Properties

    public DateTime LastLoginDate { get; set; }
    public bool IsDeleted { get; set; }
    public EUserStatus Status { get; set; }

    #endregion

    #region Relations

    public ICollection<Role> Roles { get; set; }
    public PasswordLogin PasswordLogin { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; }
    public ICollection<UserClaim> Claims { get; set; }

    #endregion
}
