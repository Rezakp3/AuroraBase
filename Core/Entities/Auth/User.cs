using Core.Entities.Relations;
using Core.Enums;

namespace Core.Entities.Auth;

public partial class User : BaseEntity<Guid>
{
    public string PhoneNo { get; set; } 

    public string? Address { get; set; }

    public string? Email { get; set; }

    public DateTime? CreateDate { get; set; } = DateTime.Now;

    public EUserStatus? Status { get; set; }

    public Guid? ParentId { get; set; }

    public string? Username { get; set; }

    public Guid? BusinessKey { get; set; }

    #region Relations

    public virtual Auth? Auth { get; set; }

    public virtual User? Parent { get; set; }
    public virtual ICollection<User> Childs { get; set; } = [];

    public virtual ICollection<Device> Devices { get; set; } = [];

    public virtual ICollection<UserClaim> UserClaims { get; set; } = [];
    public virtual ICollection<Claim> Claims { get; set; } = [];

    public virtual ICollection<UserGroup> UserGroups { get; set; } = [];
    public virtual ICollection<Group> Groups { get; set; } = [];

    public virtual ICollection<UserRole> UserRoles { get; set; } = []; 
    public virtual ICollection<Role> Roles { get; set; } = []; 

    #endregion
}
