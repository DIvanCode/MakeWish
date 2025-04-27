using MakeWish.WishService.UseCases.Features.Users.Delete;
using MakeWish.WishService.Web.Controllers.Requests.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MakeWish.WishService.Web.Controllers;

// TODO: Delete controller after switch to events from UserService handling
[ApiController]
[Route("/api/[controller]")]
public sealed class UsersController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var command = request.ToCommand();
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand(id);
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }
} 