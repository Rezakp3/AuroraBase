using System;
using System.Collections.Generic;

namespace Core.Entities.Auth;

public partial class UserClaim : BaseEntity<long>
{
    public Guid UserId { get; set; }

    public int ClaimId { get; set; }

    public string Value { get; set; } = null!;

    public virtual Claim Claim { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
