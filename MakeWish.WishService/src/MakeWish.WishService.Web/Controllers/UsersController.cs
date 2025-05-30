using MakeWish.WishService.UseCases.Abstractions.Reconciliation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MakeWish.WishService.Web.Controllers;

[ApiController]
[Route("/api/[controller]")]
public sealed class UsersController(IMediator mediator) : ControllerBase
{
    [HttpPost(":reconcile")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> ReconcileAsync(CancellationToken cancellationToken)
    {
        var command = new ReconcileUsersCommand();
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }
}