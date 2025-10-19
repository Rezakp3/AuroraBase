namespace Core.Entities.Auth.Relation;

public class UserRole
{
    public long UserId { get; set; }
    public int RoleId { get; set; }

    public User User { get; set; }
    public Role Role { get; set; }
}
