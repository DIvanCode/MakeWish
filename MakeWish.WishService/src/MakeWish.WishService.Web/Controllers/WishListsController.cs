using MakeWish.WishService.UseCases.Abstractions.Dto;
using MakeWish.WishService.UseCases.Abstractions.Features.WishLists;
using MakeWish.WishService.Web.Controllers.Requests.WishLists;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MakeWish.WishService.Web.Controllers;

[ApiController]
[Route("/api/[controller]")]
public sealed class WishListsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WishListDto>> CreateAsync(
        CreateWishListRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand();
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WishListDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var command = new GetWishListCommand(id);
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }

    [HttpGet("user/{userId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<WishListDto>>> GetUserWishListsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var command = new GetUserWishListsCommand(userId);
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WishListDto>> UpdateAsync(
        UpdateWishListRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand();
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{id:guid}/:add-wish/{wishId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WishListDto>> AddWishAsync(
        Guid id,
        Guid wishId,
        CancellationToken cancellationToken)
    {
        var command = new AddWishToWishListCommand(id, wishId);
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{id:guid}/:remove-wish/{wishId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WishListDto>> RemoveWishAsync(
        Guid id,
        Guid wishId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveWishFromWishListCommand(id, wishId);
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }

    [HttpGet("{id:guid}/users-with-access")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<UserDto>>> GetUsersWithAccessAsync(Guid id, CancellationToken cancellationToken)
    {
        var command = new GetUsersWithAccessToWishListCommand(id);
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{id:guid}/:allow-user-access/{userId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WishListDto>> AllowUserAccessAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var command = new AllowUserAccessToWishListCommand(id, userId);
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{id:guid}/:deny-user-access/{userId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WishListDto>> DenyUserAccessAsync(
        Guid id,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var command = new DenyUserAccessToWishListCommand(id, userId);
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }
} 