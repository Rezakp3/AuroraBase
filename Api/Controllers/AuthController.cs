using Application.Services.Auth;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiExplorerSettings(GroupName = "v1")]
public class AuthController(IMediator mediator) : BaseController(mediator)
{

    [HttpPost]
    public async Task<IActionResult> SendCode(SendCodeRequest request)
        => await Sender(request);
}
