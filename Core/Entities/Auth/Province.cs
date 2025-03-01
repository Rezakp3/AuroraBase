using System;
using System.Collections.Generic;

namespace Core.Entities.Auth;

public partial class Province : BaseEntity<int>
{
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<City> Cities { get; set; } = [];
}
