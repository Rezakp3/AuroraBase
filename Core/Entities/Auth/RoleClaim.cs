namespace Core.Entities.Auth;

public class RoleClaim : BaseEntity<long>
{
    public int RoleId { get; set; }
    public int ClaimId { get; set; }
    public string Value { get; set; } = null!;


    public virtual Role Role { get; set; } = null!;
    public virtual Claim Claim { get; set; } = null!;
}
