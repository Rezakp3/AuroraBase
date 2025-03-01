namespace Core.Entities.Auth;

public partial class Group : BaseEntity<int>
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public Guid? BusinessKey { get; set; }

    public virtual ICollection<UserGroup> UserGroups { get; set; } = [];
}
