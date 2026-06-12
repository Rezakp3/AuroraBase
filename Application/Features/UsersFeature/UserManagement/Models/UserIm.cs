using Application.Common.Models.Pagination;
using Core.Enums;

namespace Application.Features.UsersFeature.UserManagement.Models;

public class UserIm : PagingOption
{
    public string FName { get; set; }
    public string LName { get; set; }
    public string PhoneNumber { get; set; }
    public EUserStatus? Status { get; set; }
}
