using System;
using System.Collections.Generic;

namespace Core.Entities.Auth;

public partial class MenuService : BaseEntity<long>
{
    public long MenuId { get; set; }

    public long ServiceId { get; set; }

    public virtual Menu Menu { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
