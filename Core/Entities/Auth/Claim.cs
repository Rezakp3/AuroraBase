using Core.Entities.Relations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Auth;

public partial class Claim : BaseEntity<int>
{
    public required string Name { get; set; }
    public Guid BusinessKey { get; set; }
    public string? Type { get; set; }

    public virtual ICollection<UserClaim> UserClaims { get; set; } = [];
    public virtual ICollection<User> Users { get; set; } = [];
    public virtual ICollection<GroupClaim> GroupClaims { get; set; } = [];
    public virtual ICollection<Group> Groups { get; set; } = [];
    public virtual ICollection<RoleClaim> RoleClaims { get; set; } = [];
    public virtual ICollection<Role> Roles { get; set; } = [];


    [ForeignKey(nameof(BusinessKey))]
    public virtual Business Business { get; set; } 
}
