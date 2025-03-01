using System;
using System.Collections.Generic;

namespace Core.Entities.Auth;

public partial class UserGroup : BaseEntity<long>
{
    public Guid UserId { get; set; }

    public int GroupId { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
