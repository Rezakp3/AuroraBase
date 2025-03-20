using Core.Entities.Auth;

namespace Core.Entities.Relations;

public partial class RoleService : BaseEntity<long>
{
    public int RoleId { get; set; }

    public int ServiceId { get; set; }

    public virtual Role Role { get; set; } 

    public virtual Service Service { get; set; } 
}
