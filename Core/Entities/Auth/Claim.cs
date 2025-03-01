namespace Core.Entities.Auth;

public partial class Claim : BaseEntity<int>
{
    public required string Name { get; set; }
    public Guid BusinessKey { get; set; }
    public string? Type { get; set; }

    public virtual ICollection<UserClaim> UserClaims { get; set; } = [];
}
