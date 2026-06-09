using Api.Attributes;
using Application.Features.MenuFeature.Commands;
using Application.Features.MenuFeature.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Auth;

public class MenuController(IMediator mediator) : BaseController(mediator)
{
    [HttpPost, AutoPermission]
    public Task<IActionResult> Add([FromBody] AddMenuCommand command) => Sender(command);

    [HttpPut, AutoPermission]
    public Task<IActionResult> Update([FromBody] UpdateMenuCommand command) => Sender(command);

    [HttpPut, AutoPermission]
    public Task<IActionResult> ChangeStatus([FromBody] ChangeStatusMenuCommand command) => Sender(command);

    [HttpDelete, AutoPermission]
    public async Task<IActionResult> Delete([FromQuery] DeleteMenuCommand command) => await Sender(command);

    [HttpPut, AutoPermission]
    public Task<IActionResult> ManageServices([FromBody] ManageMenuServiceCommand command) => Sender(command);

    [HttpPut, AutoPermission]
    public Task<IActionResult> ManageRoles([FromBody] ManageMenuRoleCommand command) => Sender(command);

    [HttpGet, AutoPermission]
    public async Task<IActionResult> Search([FromQuery] SearchMenuQuery query) => await Sender(query);

    [HttpGet, AutoPermission]
    public async Task<IActionResult> DropDown([FromQuery] MenuDropDownQuery query) => await Sender(query);
}
