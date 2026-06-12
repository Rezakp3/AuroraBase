using Application.Common.Models.Pagination;

namespace Application.Features.AuthFeature.SessionManagement.Models;

public class SessionIm : PagingOption
{
    public long? UserId { get; set; }
    public string UserFName { get; set; }
    public string UserLName { get; set; }

    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
