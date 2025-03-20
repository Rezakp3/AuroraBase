using Core.Entities.Relations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Auth;

public partial class Group : BaseEntity<int>
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public Guid BusinessKey { get; set; }

    #region Relations

    public virtual ICollection<UserGroup> UserGroups { get; set; } = [];
    public virtual ICollection<User> Users { get; set; }

    public virtual ICollection<GroupClaim> GroupClaims { get; set; } = [];
    public virtual ICollection<Claim> Claims { get; set; }

    [ForeignKey(nameof(BusinessKey))]
    public virtual Business Business { get; set; }  

    #endregion
}
