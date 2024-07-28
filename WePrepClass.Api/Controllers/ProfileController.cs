using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WePrepClass.Api.Commons;
using WePrepClass.Application.UseCases.Accounts.Commands;
using WePrepClass.Application.UseCases.Accounts.Queries;
using WePrepClass.Contracts.Users;

namespace WePrepClass.Api.Controllers;

[Authorize]
public class ProfileController(ISender sender) : ApiControllerBase
{
    [HttpGet("")]
    public async Task<IActionResult> Profile()
    {
        var loginResult = await sender.Send(new GetUserProfileQuery());
        return Ok(loginResult);
    }

    [HttpPut("edit")]
    public async Task<IActionResult> Edit([FromBody] UserProfileUpdateDto learnerForUpdateDto)
    {
        var result = await sender.Send(new UpdateBasicProfileCommand(learnerForUpdateDto));
        return Ok(result);
    }

    [HttpPost("change-avatar")]
    public async Task<IActionResult> ChangeAvatar(ChangeAvatarCommand changeAvatarCommand)
    {
        var result = await sender.Send(changeAvatarCommand);
        return Ok(result);
    }
}