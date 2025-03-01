using System;
using System.Collections.Generic;

namespace Core.Entities.Auth;

public partial class LogService : BaseEntity<long>
{

    public string? Step { get; set; }

    public DateTime? CreateDate { get; set; }

    public string? RequestJson { get; set; }

    public string? ResponseJson { get; set; }
}
