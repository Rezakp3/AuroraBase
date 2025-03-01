using Core.Enums;

namespace Core.Entities.Auth;

public partial class User : BaseEntity<Guid>
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string PhoneNo { get; set; } = null!;

    public string? Image { get; set; }

    public EUserSex? Sex { get; set; }

    public string? Referrer { get; set; }

    public string? NationalCode { get; set; }

    public DateTime? BirthDate { get; set; }

    public string? Email { get; set; }

    public Guid? City { get; set; }

    public DateTime? CreateDate { get; set; }

    public EUserStatus? Status { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public string? Address { get; set; }

    public Guid? ParentId { get; set; }

    public string? Username { get; set; }

    public Guid? BusinessKey { get; set; }

    public bool? IsAuthenticated { get; set; }

    public string? AuthenticateDate { get; set; }

    public Guid? UserAuthenticated { get; set; }

    public string? NationalCodeSerial { get; set; }

    public virtual ICollection<Auth> Auths { get; set; } = new List<Auth>();

    public virtual City? CityNavigation { get; set; }

    public virtual ICollection<Device> Devices { get; set; } = new List<Device>();

    public virtual ICollection<UserClaim> UserClaims { get; set; } = new List<UserClaim>();

    public virtual ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
