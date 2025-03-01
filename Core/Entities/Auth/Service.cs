namespace Core.Entities.Auth;

public partial class Service : BaseEntity<long>
{
    public string? Name { get; set; }

    public string? Pname { get; set; }

    public string? Code { get; set; }

    public string? MicroServiceName { get; set; }

    public string? Url { get; set; }

    public string? GroupName { get; set; }

    public string? Description { get; set; }
    public Guid BusinessKey { get; set; }

    public virtual ICollection<MenuService> MenuServices { get; set; } = [];

    public virtual ICollection<RoleService> RoleServices { get; set; } = [];
}
