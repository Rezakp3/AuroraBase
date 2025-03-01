using System;
using System.Collections.Generic;

namespace Core.Entities.Auth;

public partial class InqueryUser : BaseEntity<long>
{
    public Guid UserId { get; set; }

    public int? InquiryMobileOwnerShip { get; set; }

    public int? InquiryPerson { get; set; }

    public int? InquiryAddress { get; set; }

    public int? InquirySendVideo { get; set; }

    public string? InquirySendVideoMessage { get; set; }
}
