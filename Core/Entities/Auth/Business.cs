namespace Core.Entities.Auth;

public class Business : BaseEntity<long>
{
    public required string Name { get; set; }
    public Guid BusinessKey { get; set; }
    public DateTime CreateDate { get; set; }
    public bool IsAdmin { get; set; }
}
