using Api.Attributes;
using Application.Features.UsersFeature.ClaimManagement.Commands;
using Application.Features.UsersFeature.ClaimManagement.Queries;
using Application.Features.UsersFeature.UserManagement.Commands;
using Application.Features.UsersFeature.UserManagement.Queries;
using Application.Features.UsersFeature.UserRelations.Commands;
using Application.Features.UsersFeature.UserRelations.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Auth;

public class UserController(IMediator mediator) : BaseController(mediator)
{

    [HttpPost, AutoPermission]
    public async Task<IActionResult> Add([FromBody] CreateUserCommand command)
        => await Sender(command);

    [HttpPut, AutoPermission]
    public async Task<IActionResult> ChangeStatus([FromBody] ChangeUserStatusCommand command)
        => await Sender(command);

    [HttpGet, AutoPermission]
    public async Task<IActionResult> Search([FromQuery] UserSearchQuery query)
        => await Sender(query);

    #region manage role

    [HttpGet, AutoPermission]
    public async Task<IActionResult> UserRoleDropDown([FromQuery] UserRoleDropDownQuery command)
        => await Sender(command);

    [HttpPut, AutoPermission]
    public async Task<IActionResult> ManageRoles([FromBody] ManageUserRoleCommand command)
        => await Sender(command);

    #endregion

    #region manage claims

    [HttpPost, AutoPermission]
    public async Task<IActionResult> AddClaim([FromBody] AddUserClaimCommand command)
        => await Sender(command);

    [HttpPut, AutoPermission]
    public async Task<IActionResult> UpdateUserClaim([FromBody] UpdateUserClaimCommand command)
        => await Sender(command);

    [HttpDelete, AutoPermission]
    public async Task<IActionResult> DeleteUserClaim([FromQuery] DeleteUserClaimCommand query)
        => await Sender(query);

    [HttpGet, AutoPermission]
    public async Task<IActionResult> GetUserClaim([FromQuery] GetUserClaimsQuery query)
        => await Sender(query);

    #endregion
}