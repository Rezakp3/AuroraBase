using Api.Attributes;
using Application.Features.ServiceFeatures.Commands;
using Application.Features.ServiceFeatures.Queries;
using Application.Features.ServiceFeatures.ServiceRelation.Commands;
using Application.Features.ServiceFeatures.ServiceRelation.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Auth;

public class ServiceController(IMediator mediator) : BaseController(mediator)
{
    [HttpPost, AutoPermission]
    public async Task<IActionResult> Add([FromBody] AddServiceCommand command) 
        => await Sender(command);

    [HttpPut, AutoPermission]
    public async Task<IActionResult> Update([FromBody] UpdateServiceCommand command) 
        => await Sender(command);

    [HttpDelete, AutoPermission]
    public async Task<IActionResult> Delete([FromQuery] DeleteServiceCommand command)
        => await Sender(command);

    [HttpGet, AutoPermission]
    public async Task<IActionResult> Search([FromQuery] SearchServiceQuery query) 
        => await Sender(query);

    [HttpGet, AutoPermission]
    public async Task<IActionResult> DropDown([FromQuery] ServiceDropDownQuery query) 
        => await Sender(query);

    [HttpGet, AutoPermission]
    public async Task<IActionResult> RolesDropDown([FromQuery] ServiceRoleDropDownQuery query) 
        => await Sender(query);

    [HttpPut, AutoPermission]
    public async Task<IActionResult> ManageRoles([FromBody] ManageServiceRolesCommand command) 
        => await Sender(command);
}
