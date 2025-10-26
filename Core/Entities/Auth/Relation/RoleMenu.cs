namespace Core.Entities.Auth.Relation;

public class RoleMenu
{
    public int RoleId { get; set; } 
    public int MenuId { get; set; }

    public Role Role { get; set; }
    public Menu Menu { get; set; }
}
