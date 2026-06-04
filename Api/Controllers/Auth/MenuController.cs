using Api.Attributes;
using Application.Features.MenuFeature.Commands;
using Application.Features.MenuFeature.Queries;
using Application.Features.RoleFeatures.RoleClaimFeatures.Commands;
using Application.Features.RoleFeatures.RoleManagement.Commands;
using Application.Features.RoleFeatures.RoleManagement.Queries;
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
    public Task<IActionResult> Delete([FromBody] DeleteMenuCommand command) => Sender(command);

    [HttpGet, AutoPermission]
    public Task<IActionResult> Search([FromQuery] SearchMenuQuery command) => Sender(command);
}
