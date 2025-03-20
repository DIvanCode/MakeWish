using MakeWish.UserService.UseCases.Dto;
using MakeWish.UserService.UseCases.Features.Friendships.GetConfirmedFriendships;
using MakeWish.UserService.UseCases.Features.Friendships.GetPendingFriendshipsFromUser;
using MakeWish.UserService.UseCases.Features.Friendships.GetPendingFriendshipsToUser;
using MakeWish.UserService.UseCases.Features.Users.Delete;
using MakeWish.UserService.UseCases.Features.Users.GetById;
using MakeWish.UserService.Web.Controllers.Requests;
using MakeWish.UserService.Web.Controllers.Requests.Friendships;
using MakeWish.UserService.Web.Controllers.Requests.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MakeWish.UserService.Web.Controllers;

[ApiController]
[Route("/api/[controller]")]
public sealed class Friendships(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FriendshipDto>> CreateAsync(CreateFriendshipRequest request, CancellationToken cancellationToken)
    {
        var command = request.ToCommand();
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }
    
    [HttpPost(":confirm")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FriendshipDto>> ConfirmAsync(ConfirmFriendshipRequest request, CancellationToken cancellationToken)
    {
        var command = request.ToCommand();
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }

    [HttpDelete]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveAsync(RemoveFriendshipRequest request, CancellationToken cancellationToken)
    {
        var command = request.ToCommand();
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }
    
    [HttpGet("confirmed/{userId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<FriendshipDto>>> GetConfirmedAsync(Guid userId, CancellationToken cancellationToken)
    {
        var command = new GetConfirmedFriendshipsCommand(userId);
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }
    
    [HttpGet("pending/from/{userId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<FriendshipDto>>> GetPendingFromAsync(Guid userId, CancellationToken cancellationToken)
    {
        var command = new GetPendingFriendshipsFromUserCommand(userId);
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }
    
    [HttpGet("pending/to/{userId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<FriendshipDto>>> GetPendingToAsync(Guid userId, CancellationToken cancellationToken)
    {
        var command = new GetPendingFriendshipsToUserCommand(userId);
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }
}