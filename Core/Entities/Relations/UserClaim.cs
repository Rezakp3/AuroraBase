using Core.Entities.Auth;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Relations;

public partial class UserClaim : BaseEntity<long>
{
    public Guid UserId { get; set; }

    public int ClaimId { get; set; }

    [MaxLength(50)]
    public string Value { get; set; } 

    public virtual Claim Claim { get; set; } 

    public virtual User User { get; set; } 
}
