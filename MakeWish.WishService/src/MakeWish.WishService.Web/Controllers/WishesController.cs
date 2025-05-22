using MakeWish.WishService.UseCases.Dto;
using MakeWish.WishService.UseCases.Features.Wishes.Get;
using MakeWish.WishService.UseCases.Features.Wishes.Delete;
using MakeWish.WishService.UseCases.Features.Wishes.Complete;
using MakeWish.WishService.UseCases.Features.Wishes.CompleteApprove;
using MakeWish.WishService.UseCases.Features.Wishes.CompleteReject;
using MakeWish.WishService.UseCases.Features.Wishes.Promise;
using MakeWish.WishService.UseCases.Features.Wishes.PromiseCancel;
using MakeWish.WishService.UseCases.Features.Wishes.Restore;
using MakeWish.WishService.UseCases.Features.Wishes.GetPromisedWishes;
using MakeWish.WishService.UseCases.Features.Wishes.GetUserWishes;
using MakeWish.WishService.UseCases.Features.Wishes.GetWaitingApproveWishes;
using MakeWish.WishService.Web.Controllers.Requests.Wishes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MakeWish.WishService.Web.Controllers;

[ApiController]
[Route("/api/[controller]")]
public sealed class WishesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WishDto>> CreateAsync(CreateWishRequest request, CancellationToken cancellationToken)
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
    public async Task<ActionResult<WishDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var command = new GetWishCommand(id);
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }
    
    [HttpGet("user/{userId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<WishDto>>> GetUserWishesAsync(
        Guid userId,
        [FromQuery] string? query,
        CancellationToken cancellationToken)
    {
        var command = new GetUserWishesCommand(userId, query);
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPut]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WishDto>> UpdateAsync(UpdateWishRequest request, CancellationToken cancellationToken)
    {
        var command = request.ToCommand();
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteWishCommand(id);
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }
    
    [HttpPost("{id:guid}/:restore")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WishDto>> RestoreAsync(Guid id, CancellationToken cancellationToken)
    {
        var command = new RestoreWishCommand(id);
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{id:guid}/:complete")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WishDto>> CompleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var command = new CompleteWishCommand(id);
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{id:guid}/:complete-approve")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WishDto>> CompleteApproveAsync(Guid id, CancellationToken cancellationToken)
    {
        var command = new CompleteApproveWishCommand(id);
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }
    
    [HttpPost("{id:guid}/:complete-reject")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WishDto>> CompleteRejectAsync(Guid id, CancellationToken cancellationToken)
    {
        var command = new CompleteRejectWishCommand(id);
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{id:guid}/:promise")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WishDto>> PromiseAsync(Guid id, CancellationToken cancellationToken)
    {
        var command = new PromiseWishCommand(id);
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }

    [HttpPost("{id:guid}/:promise-cancel")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WishDto>> PromiseCancelAsync(Guid id, CancellationToken cancellationToken)
    {
        var command = new PromiseCancelWishCommand(id);
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }

    [HttpGet("promised")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<WishDto>>> GetPromisedWishesAsync(CancellationToken cancellationToken)
    {
        var command = new GetPromisedWishesCommand();
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }

    [HttpGet("waiting-approve")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<WishDto>>> GetWaitingApproveWishesAsync(CancellationToken cancellationToken)
    {
        var command = new GetWaitingApproveWishesCommand();
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }
} 