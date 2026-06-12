using Core.Enums;
using Utils.Helpers;

namespace Application.Features.UsersFeature.UserManagement.Models;

public class UserDto
{
    public long Id { get; set; }
    public string FName { get; set; }
    public string LName { get; set; }
    public string PhoneNumber { get; set; }
    public EUserStatus Status { get; set; }
    public string StatusStr => Status.GetDescription();
    public string LastLogin { get; set; }
}
