using Core.Entities.Auth;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Relations;

public class RoleClaim : BaseEntity<long>
{
    public int RoleId { get; set; }
    public int ClaimId { get; set; }
    [MaxLength(50)]
    public string Value { get; set; } 


    public virtual Role Role { get; set; } 
    public virtual Claim Claim { get; set; } 
}
