namespace Core.Entities.Auth;

public partial class Role : BaseEntity<int>
{
    public string? Name { get; set; }
    public string? Pname { get; set; }
    public string? Description { get; set; }
    public bool? IsAdmin { get; set; }
    public Guid? BusinessKey { get; set; }

    public virtual ICollection<RoleService> RoleServices { get; set; } = [];

    public virtual ICollection<UserRole> UserRoles { get; set; } = [];
}
