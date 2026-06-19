using Api.Attributes;
using Application.Features.RoleFeatures.RoleClaimFeatures.Commands;
using Application.Features.RoleFeatures.RoleClaimFeatures.Queries;
using Application.Features.RoleFeatures.RoleManagement.Commands;
using Application.Features.RoleFeatures.RoleManagement.Queries;
using Application.Features.RoleFeatures.RoleRelations.Commands;
using Application.Features.RoleFeatures.RoleRelations.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Auth;

public class RoleController(IMediator mediator) : BaseController(mediator)
{
    [HttpPost, AutoPermission]
    public async Task<IActionResult> Add([FromBody] AddRoleCommand command) => await Sender(command);

    [HttpPut, AutoPermission]
    public async Task<IActionResult> Update([FromBody] UpdateRoleCommand command) => await Sender(command);

    [HttpDelete, AutoPermission]
    public async Task<IActionResult> Delete([FromQuery] DeleteRoleCommand command) => await Sender(command);

    [HttpGet, AutoPermission]
    public Task<IActionResult> Search([FromQuery] SearchRoleQuery command) => Sender(command);

    [HttpGet, AutoPermission]
    public async Task<IActionResult> DropDown([FromQuery] RoleDropDownQuery query) => await Sender(query);


    [HttpGet, AutoPermission]
    public async Task<IActionResult> ServiceDropDown([FromQuery] RoleServiceDropDownQuery query) => await Sender(query);

    [HttpPut, AutoPermission]
    public async Task<IActionResult> ManageServices([FromBody] ManageRoleServiceCommand command) => await Sender(command);


    [HttpGet, AutoPermission]
    public async Task<IActionResult> MenuDropDown([FromQuery] RoleMenuDropDownQuery query) => await Sender(query);

    [HttpPut, AutoPermission]
    public async Task<IActionResult> ManageMenus([FromBody] ManageRoleMenusCommand command) => await Sender(command);

    [HttpPost, AutoPermission]
    public async Task<IActionResult> AddClaim([FromBody] AddRoleClaimCommand command) => await Sender(command);

    [HttpPut, AutoPermission]
    public async Task<IActionResult> UpdateClaim([FromBody] UpdateRoleClaimCommand command) => await Sender(command);

    [HttpDelete, AutoPermission]
    public async Task<IActionResult> DeleteClaim([FromQuery] DeleteRoleClaimCommand command) => await Sender(command);
    
    [HttpGet, AutoPermission]
    public async Task<IActionResult> GetRoleClaims([FromQuery] GetRoleClaimsQuery command) => await Sender(command);

}
