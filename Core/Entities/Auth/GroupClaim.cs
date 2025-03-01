namespace Core.Entities.Auth;

public class GroupClaim : BaseEntity<long>
{
    public int GroupId { get; set; }
    public int ClaimId { get; set; }
    public string Value { get; set; } = null!;

    public virtual Claim Claim { get; set; } = null!;
    public virtual Group Group { get; set; } = null!;
}
