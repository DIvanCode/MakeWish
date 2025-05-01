using FluentResults;
using MakeWish.UserService.Interfaces.DataAccess;
using MakeWish.UserService.Models.Entities;
using MakeWish.UserService.UseCases.Services;
using MakeWish.UserService.Utils.Errors;
using MediatR;

namespace MakeWish.UserService.UseCases.Features.Friendships.Remove;

public sealed class RemoveFriendshipHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    : IRequestHandler<RemoveFriendshipCommand, Result>
{
    public async Task<Result> Handle(
        RemoveFriendshipCommand request,
        CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated)
        {
            return new AuthenticationError();
        }

        if (request.FirstUserId != userContext.UserId && request.SecondUserId != userContext.UserId)
        {
            return new ForbiddenError(
                nameof(Friendship), 
                "remove", 
                "users", 
                $"{request.FirstUserId} and {request.SecondUserId}");
        }
        
        var firstUser = await unitOfWork.Users.GetByIdAsync(request.FirstUserId, cancellationToken);
        if (firstUser is null)
        {
            return new EntityNotFoundError(nameof(User), nameof(User.Id), request.FirstUserId);
        }
        
        var secondUser = await unitOfWork.Users.GetByIdAsync(request.SecondUserId, cancellationToken);
        if (secondUser is null)
        {
            return new EntityNotFoundError(nameof(User), nameof(User.Id), request.SecondUserId);
        }
        
        var friendship = await unitOfWork.Friendships.GetBetweenUsersAsync(firstUser, secondUser, cancellationToken);
        if (friendship is null)
        {
            return new EntityNotFoundError(nameof(Friendship), "such users");
        }
        
        var removeResult = friendship.RemoveBy(by: userContext.UserId == firstUser.Id ? firstUser : secondUser);
        if (removeResult.IsFailed)
        {
            return removeResult;
        }
        
        unitOfWork.Friendships.Remove(friendship);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}