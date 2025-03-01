using Core.Enums;

namespace Core.Entities.Auth;

public partial class Device : BaseEntity<long>
{
    public Guid UserId { get; set; }

    public string? FirebaseToken { get; set; }

    public EPlatform? Platform { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpirationDate { get; set; }

    public EChannel? Channel { get; set; }

    public EMedia? Media { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreateDate { get; set; }

    public virtual User User { get; set; } = null!;
}
