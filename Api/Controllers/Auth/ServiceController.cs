using Api.Attributes;
using Application.Features.ServiceFeatures.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Auth;

public class ServiceController(IMediator mediator) : BaseController(mediator)
{
    [HttpPost, AutoPermission]
    public Task<IActionResult> Add([FromBody] AddServiceCommand command) => Sender(command);
   
    [HttpPut, AutoPermission]
    public Task<IActionResult> Update([FromBody] UpdateServiceCommand command) => Sender(command);
   
    [HttpDelete, AutoPermission]
    public Task<IActionResult> Delete([FromBody] DeleteServiceCommand command) => Sender(command);
}
