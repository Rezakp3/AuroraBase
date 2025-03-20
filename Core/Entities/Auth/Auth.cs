using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Auth;

public partial class Auth : BaseEntity<long>
{
    public string PhoneNo { get; set; } 
    public Guid BusinessKey { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.Now;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpirationDate { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } 
    [ForeignKey(nameof(BusinessKey))]
    public virtual Business Business { get; set; } 
}
