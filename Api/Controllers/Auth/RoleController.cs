using Api.Attributes;
using Application.Features.RoleFeatures.RoleClaimFeatures.Commands;
using Application.Features.RoleFeatures.RoleManagement.Commands;
using Application.Features.RoleFeatures.RoleManagement.Queries;
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

    [HttpPost, AutoPermission]
    public async Task<IActionResult> AssignServices([FromBody] AssignServicesToRoleCommand command) => await Sender(command);

    [HttpPost, AutoPermission]
    public async Task<IActionResult> AssignMenues([FromBody] AssignMenuToRoleCommand command) => await Sender(command);

    [HttpPost, AutoPermission]
    public async Task<IActionResult> AddClaims([FromBody] AddClaimsToRoleCommand command) => await Sender(command);

    [HttpGet, AutoPermission]
    public Task<IActionResult> Search([FromQuery] SearchRoleQuery command) => Sender(command);
    [HttpGet, AutoPermission]
    public async Task<IActionResult> DropDown([FromQuery] RoleDropDownQuery query) => await Sender(query);
}
