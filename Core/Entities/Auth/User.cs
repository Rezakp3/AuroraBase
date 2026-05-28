using Core.Enums;
using Core.Entities.Auth.Relation;

namespace Core.Entities.Auth;

public class User : BaseEntityWithDate<long>
{
    #region Properties

    public DateTime LastLoginDate { get; set; }
    public EUserStatus Status { get; set; }
    public string FName { get; set; }
    public string LName { get; set; }

    #endregion

    #region Otp Login

    public string PhoneNumber { get; set; }
    public string OtpCode { get; set; }
    public int TryCount { get; set; }
    public DateTime? OtpExpireDate { get; set; }

    #endregion

    #region UserName Password

    public string UserName { get; set; }
    public string PasswordHash { get; set; }
    public DateTime? LastUpdateDate { get; set; }
    public string ResetPasswordToken { get; set; }
    public DateTime? ResetPasswordTokenExpireDate { get; set; }

    #endregion

    #region Relations

    public ICollection<UserRole> UserRoles { get; set; } = [];
    public ICollection<Session> Sessions { get; set; } = [];
    public ICollection<UserClaim> Claims { get; set; } = [];

    #endregion
}
