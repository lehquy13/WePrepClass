using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WePrepClass.Api.Commons;
using WePrepClass.Application.UseCases.Administrator.Users.Commands;
using WePrepClass.Application.UseCases.Administrator.Users.Queries;

namespace WePrepClass.Api.Controllers;

[Authorize]
public class UserController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(GetUsersQuery query)
    {
        var result = await sender.Send(query);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await sender.Send(new GetUserByIdQuery(id));

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserCommand command)
    {
        var result = await sender.Send(command);

        return Ok(result);
    }
}