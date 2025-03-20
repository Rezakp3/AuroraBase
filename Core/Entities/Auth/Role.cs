using Core.Entities.Relations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Auth;

public partial class Role : BaseEntity<int>
{
    public string Name { get; set; }
    public string Pname { get; set; }
    public string? Description { get; set; }
    public bool IsAdmin { get; set; }
    public Guid BusinessKey { get; set; }

    public virtual ICollection<RoleService> RoleServices { get; set; } = [];
    public virtual ICollection<Service> Services { get; set; } = [];
    public virtual ICollection<UserRole> UserRoles { get; set; } = [];
    public virtual ICollection<User> Users { get; set; } = [];
    public virtual ICollection<RoleClaim>  RoleClaims { get; set; } = [];
    public virtual ICollection<Claim>  Claims { get; set; } = [];
    
    [ForeignKey(nameof(BusinessKey))]
    public virtual Business Business { get; set; }
}
