namespace Core.Entities.Auth;

public class Business : BaseEntity<Guid>
{
    public string Name { get; set; }
    public DateTime? CreateDate { get; set; } = DateTime.Now;
    public int ExpireTimeInMinute { get; set; }
    public bool IsActive { get; set; }
    public string PrivateKey { get; set; }
    public string PublicKey { get; set; }
    public bool IsAdmin { get; set; }

    public ICollection<Auth> Auths { get; set; } = [];
    public ICollection<Claim> Claims { get; set; } = [];
    public ICollection<Group> Groups { get; set; } = [];
    public ICollection<Menu> Menues { get; set; } = [];
    public ICollection<Service> Services { get; set; } = [];
}
