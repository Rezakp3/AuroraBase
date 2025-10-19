using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Auth;

public class RefreshToken : BaseEntity<Guid>
{
    #region Properties

    public DateTime ExpireDate { get; set; }
    public bool IsRevoked { get; set; }
    public string? DeviceName { get; set; }

    // متد کمکی برای بررسی انقضا
    [NotMapped] 
    public bool IsExpired => DateTimeOffset.UtcNow >= ExpireDate;

    #endregion

    #region relation

    public long UserId { get; set; }
    public User User { get; set; } = null!;

    #endregion

    public void Revoke() => IsRevoked = true;
}
