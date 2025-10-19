namespace Core.Entities.Auth.Relation;

public class RoleService : BaseEntity<int>
{
    public int RoleId { get; set; }
    public int ServiceId { get; set; }
    public Role Role { get; set; }
    public Service Service { get; set; }
}
