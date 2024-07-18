using WePrepClass.Api.Commons;
using Matt.SharedKernel.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WePrepClass.Application.UseCases.Users.Commands;
using WePrepClass.Application.UseCases.Users.Queries;

namespace WePrepClass.Api.Controllers;

[Authorize]
public class UserController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await sender.Send(new GetAllUserQuery());
        
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