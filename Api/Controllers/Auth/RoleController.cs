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
    public Task<IActionResult> Add([FromBody] AddRoleCommand command) => Sender(command);
   
    [HttpPut, AutoPermission]
    public Task<IActionResult> Update([FromBody] UpdateRoleCommand command) => Sender(command);
   
    [HttpDelete, AutoPermission]
    public Task<IActionResult> Delete([FromBody] DeleteRoleCommand command) => Sender(command);

    [HttpPost, AutoPermission]
    public Task<IActionResult> AssignServices([FromBody] AssignServicesToRoleCommand command) => Sender(command);

    [HttpPost, AutoPermission]
    public Task<IActionResult> AssignMenues([FromBody] AssignMenuToRoleCommand command) => Sender(command);

    [HttpPost, AutoPermission]
    public Task<IActionResult> AddClaims([FromBody] AddClaimsToRoleCommand command) => Sender(command);

    [HttpGet, AutoPermission]
    public Task<IActionResult> Search([FromQuery] SearchRoleQuery command) => Sender(command);
}
