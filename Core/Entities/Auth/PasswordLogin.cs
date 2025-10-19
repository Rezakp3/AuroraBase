namespace Core.Entities.Auth;

public class PasswordLogin : BaseEntityWithDate<int>
{
    #region Email 

    public string Email { get; set; }
    public bool EmailIsVerified { get; set; }
    public string EmailVerificationCode { get; set; }
    public DateTime? VerifyCodeExpireDate { get; set; }

    #endregion

    public string UserName { get; set; }
    public string PasswordHash { get; set; }
    public DateTime LastUpdateDate { get; set; }


    public int UserId { get; set; }
    public User User { get; set; }
}
