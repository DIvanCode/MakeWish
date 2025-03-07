using FluentResults;
using MakeWish.UserService.Interfaces.DataAccess;
using MakeWish.UserService.Models;
using MakeWish.UserService.UseCases.Dto;
using MakeWish.UserService.UseCases.Services;
using MakeWish.UserService.Utils.Errors;
using MediatR;

namespace MakeWish.UserService.UseCases.Features.Users.Delete;

public sealed class DeleteHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    : IRequestHandler<DeleteCommand, Result>
{
    public async Task<Result> Handle(DeleteCommand request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated)
        {
            return new AuthenticationError();
        }
        
        if (request.UserId != userContext.UserId)
        {
            return new ForbiddenError(
                nameof(User), 
                "delete", 
                nameof(User.Id), 
                request.UserId);
        }
        
        var user = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return new EntityNotFoundError(nameof(User), nameof(User.Id), request.UserId);
        }
        
        unitOfWork.Users.Remove(user);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}