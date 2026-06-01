using Api.Attributes;
using Application.Features.SettingFeatures.Commands;
using Application.Features.SettingFeatures.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class SettingController(IMediator mediator) : BaseController(mediator)
{
    [HttpGet, AutoPermission]
    public Task<IActionResult> GetAll() => Sender(new GetAllSettingsQuery());


    [HttpPost, AutoPermission]
    public Task<IActionResult> Update([FromBody] UpdateSettingCommand command) => Sender(command);

}
