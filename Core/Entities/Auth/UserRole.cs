namespace Core.Entities.Auth;

public partial class UserRole : BaseEntity<long>
{
    public Guid UserId { get; set; }
    public int RoleId { get; set; }


    public virtual Role Role { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
