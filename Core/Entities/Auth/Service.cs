using Core.Entities.Relations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Auth;

public partial class Service : BaseEntity<int>
{
    public string Name { get; set; }

    public string Pname { get; set; }

    public string? MicroServiceName { get; set; }

    public string? Description { get; set; }
    public Guid BusinessKey { get; set; }

    [ForeignKey(nameof(BusinessKey))]
    public virtual Business Business { get; set; } 

    public virtual ICollection<MenuService> MenuServices { get; set; } = [];
    public virtual ICollection<Menu> Menues { get; set; } = [];

    public virtual ICollection<RoleService> RoleServices { get; set; } = [];
    public virtual ICollection<Role> Roles { get; set; } = [];
}
