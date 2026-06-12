using DNTPersianUtils.Core;

namespace Application.Features.AuthFeature.SessionManagement.Models;

public class SessionDto
{
    public Guid Id { get; set; }
    public DateTime ExpireDate { get; set; }
    public string ExpireDateStr => ExpireDate.ToShortPersianDateString();
    public DateTime CreateDate { get; set; }
    public string CreateDateStr => CreateDate.ToShortPersianDateString();
    public bool IsRevoked { get; set; }
    public string DeviceName { get; set; }
    public string UserFName { get; set; }
    public string UserLName { get; set; }
}
