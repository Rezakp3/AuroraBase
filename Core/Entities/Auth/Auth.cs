namespace Core.Entities.Auth;

public partial class Auth : BaseEntity<long>
{
    public string PhoneNo { get; set; } = null!;
    public Guid BusinessKey { get; set; }

    public Guid UserId { get; set; }
    public int? LoginCode { get; set; }
    public int LoginCodeTryCount { get; set; }
    public DateTime? LoginCodeExpirationDate { get; set; }
    public DateTime? CreateDate { get; set; }
    public string? Password { get; set; }
    public string? Salt { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpirationDate { get; set; }

    public virtual User User { get; set; } = null!;
}
