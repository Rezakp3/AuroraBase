using Api.Attributes;
using Application.Features.MenuFeature.MenuManagement.Commands;
using Application.Features.MenuFeature.MenuManagement.Queries;
using Application.Features.MenuFeature.MenuRelations.Commands;
using Application.Features.MenuFeature.MenuRelations.Queries;
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

    [HttpGet, AutoPermission]
    public async Task<IActionResult> Search([FromQuery] SearchMenuQuery query) => await Sender(query);

    [HttpGet, AutoPermission]
    public async Task<IActionResult> DropDown([FromQuery] MenuDropDownQuery query) => await Sender(query);

    [HttpGet, AutoPermission(true)]
    public async Task<IActionResult> MenuVersion([FromQuery] MenuVersionQuery query) => await Sender(query);
    
    [HttpGet, AutoPermission(true)]
    public async Task<IActionResult> SideBar([FromQuery] SideBarMenuQuery query) => await Sender(query);



    [HttpGet, AutoPermission]
    public async Task<IActionResult> MenuServiceDropDown([FromQuery] MenuServiceDropDownQuery query) => await Sender(query);

    [HttpPut, AutoPermission]
    public Task<IActionResult> ManageServices([FromBody] ManageMenuServiceCommand command) => Sender(command);

    [HttpGet, AutoPermission]
    public async Task<IActionResult> MenuRoleDropDown([FromQuery] MenuRoleDropDownQuery query) => await Sender(query);

    [HttpPut, AutoPermission]
    public Task<IActionResult> ManageRoles([FromBody] ManageMenuRoleCommand command) => Sender(command);

}
