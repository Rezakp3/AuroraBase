namespace Core.Entities.Auth;

public partial class RoleService : BaseEntity<long>
{
    public int RoleId { get; set; }

    public int ServiceId { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
