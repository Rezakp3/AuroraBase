using Core.Entities.Relations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Auth;

public partial class Menu : BaseEntity<long>
{
    public string Name { get; set; } 

    public string Pname { get; set; } 

    public string? Url { get; set; }

    public long? ParentId { get; set; }

    public string? Icon { get; set; }

    public int? Priority { get; set; }

    public Guid? BusinessKey { get; set; }

    [ForeignKey(nameof(BusinessKey))]
    public virtual Business Business { get; set; }
    public Menu? Parent { get; set; }
    public ICollection<Menu> Childs { get; set; } = [];
    public virtual ICollection<MenuService> MenuServices { get; set; } = [];
    public ICollection<Service> Services { get; set; } = [];
}
