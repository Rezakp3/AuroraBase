namespace Core.Entities.Auth;

public partial class Menu : BaseEntity<long>
{
    public string Name { get; set; } = null!;

    public string Pname { get; set; } = null!;

    public string? Url { get; set; }

    public long? ParentId { get; set; }

    public string? Icon { get; set; }

    public int? Priority { get; set; }

    public Guid? BusinessKey { get; set; }

    public virtual ICollection<MenuService> MenuServices { get; set; } = [];
}
