using System;
using System.Collections.Generic;

namespace Core.Entities.Auth;

public partial class City : BaseEntity<int>
{
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int ProvinceId { get; set; }

    public virtual Province Province { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = [];
}
