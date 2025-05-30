using MakeWish.UserService.UseCases.Dto;
using MakeWish.UserService.UseCases.Features.Users.Delete;
using MakeWish.UserService.UseCases.Features.Users.GetAll;
using MakeWish.UserService.UseCases.Features.Users.GetById;
using MakeWish.UserService.UseCases.Features.Users.GetCurrent;
using MakeWish.UserService.Web.Controllers.Requests.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MakeWish.UserService.Web.Controllers;

[ApiController]
[Route("/api/[controller]")]
public sealed class UsersController(IMediator mediator) : ControllerBase
{
    [HttpPost(":register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserDto>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var command = request.ToCommand();
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }
    
    [HttpPost(":authenticate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<string>> AuthenticateAsync(AuthenticateRequest request, CancellationToken cancellationToken)
    {
        var command = request.ToCommand();
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }
    
    [HttpGet(":current")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetCurrentAsync(CancellationToken cancellationToken)
    {
        var command = new GetCurrentCommand();
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }
    
    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var command = new GetByIdCommand(id);
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }
    
    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UserDto>>> GetAllAsync(
        [FromQuery] string? query,
        [FromQuery] bool? onlyFriends,
        CancellationToken cancellationToken)
    {
        var command = new GetAllUsersCommand(query, onlyFriends);
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
        var command = new DeleteCommand(id);
        var result = await mediator.Send(command, cancellationToken);
        return this.HandleResult(result);
    }
}