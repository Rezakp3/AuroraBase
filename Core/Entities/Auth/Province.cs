using System;
using System.Collections.Generic;

namespace Core.Entities.Auth;

public partial class Province : BaseEntity<int>
{
    public string Code { get; set; } 

    public string Name { get; set; } 

    public virtual ICollection<City> Cities { get; set; } = [];
}
