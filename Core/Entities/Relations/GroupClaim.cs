using Core.Entities.Auth;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Relations;

public class GroupClaim : BaseEntity<long>
{
    public int GroupId { get; set; }
    public int ClaimId { get; set; }
    [MaxLength(50)]
    public string Value { get; set; } 

    public virtual Claim Claim { get; set; } 
    public virtual Group Group { get; set; } 
}
