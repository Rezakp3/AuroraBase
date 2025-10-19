using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Contracts;

namespace Api.Controllers.Auth;


public class AuthController(IMediator mediator) : BaseController(mediator)
{
}
