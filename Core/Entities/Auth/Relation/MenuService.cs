namespace Core.Entities.Auth.Relation;

public class MenuService
{
    public int MenuId { get; set; }
    public int ServiceId { get; set; }

    public Menu Menu { get; set; }
    public Service Service { get; set; }
}
