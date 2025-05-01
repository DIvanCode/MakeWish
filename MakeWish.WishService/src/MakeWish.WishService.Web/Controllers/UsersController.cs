using MakeWish.WishService.UseCases.Reconciliation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MakeWish.WishService.Web.Controllers;

[ApiController]
[Route("/api/[controller]")]
public sealed class UsersController(IMediator mediator) : ControllerBase
{
    [HttpPost(":reconcile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> ReconcileUsersAsync(CancellationToken cancellationToken)
    {
        var command = new ReconcileUsersCommand();
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }
    
    [HttpPost("friendships/:reconcile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> ReconcileFriendshipsAsync(CancellationToken cancellationToken)
    {
        var command = new ReconcileFriendshipsCommand();
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }
}