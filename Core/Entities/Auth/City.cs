namespace Core.Entities.Auth;

public partial class City : BaseEntity<int>
{
    public string Code { get; set; } 

    public string Name { get; set; } 

    public int ProvinceId { get; set; }

    public virtual Province Province { get; set; } 

    public virtual ICollection<Profile> Profiles { get; set; } = [];
}
