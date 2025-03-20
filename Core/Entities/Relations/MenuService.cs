using Core.Entities.Auth;
using System;
using System.Collections.Generic;

namespace Core.Entities.Relations;

public partial class MenuService : BaseEntity<long>
{
    public long MenuId { get; set; }

    public int ServiceId { get; set; }

    public virtual Menu Menu { get; set; } 

    public virtual Service Service { get; set; } 
}
