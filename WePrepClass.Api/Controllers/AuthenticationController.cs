using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WePrepClass.Api.Commons;
using WePrepClass.Application.UseCases.Accounts.Commands;
using WePrepClass.Application.UseCases.Accounts.Queries;

namespace WePrepClass.Api.Controllers;

public class AuthenticationController(ISender mediator) : ApiControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginQuery query)
    {
        var result = await mediator.Send(query);
        return Ok(result);
    }

    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand changePasswordCommand)
    {
        var result = await mediator.Send(changePasswordCommand);
        return Ok(result);
    }
}