﻿using MakeWish.UserService.UseCases.Dto;
using MakeWish.UserService.UseCases.Features.Friendships.GetAll;
using MakeWish.UserService.UseCases.Features.Friendships.GetConfirmed;
using MakeWish.UserService.UseCases.Features.Friendships.GetPendingFriendshipsToUser;
using MakeWish.UserService.UseCases.Features.Friendships.GetPendingFromUser;
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
[Authorize]
[Route("/api/[controller]")]
public sealed class Friendships(IMediator mediator) : ControllerBase
{
    [HttpPost]
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
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<FriendshipDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var command = new GetAllFriendshipsCommand();
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }
    
    [HttpPost(":confirm")]
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
    
    [HttpGet("pending-from/{userId:guid}")]
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
    
    [HttpGet("pending-to/{userId:guid}")]
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