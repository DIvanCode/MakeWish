using FluentResults;
using MakeWish.UserService.Interfaces.DataAccess;
using MakeWish.UserService.Models;
using MakeWish.UserService.UseCases.Dto;
using MakeWish.UserService.UseCases.Services;
using MakeWish.UserService.Utils.Errors;
using MediatR;

namespace MakeWish.UserService.UseCases.Features.Friendships.ConfirmFriendship;

public sealed class ConfirmFriendshipHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    : IRequestHandler<ConfirmFriendshipCommand, Result<FriendshipDto>>
{
    public async Task<Result<FriendshipDto>> Handle(
        ConfirmFriendshipCommand request,
        CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated)
        {
            return new AuthenticationError();
        }

        if (request.SecondUserId != userContext.UserId)
        {
            return new ForbiddenError(
                nameof(Friendship), 
                "confirm", 
                nameof(Friendship.SecondUser), 
                request.SecondUserId);
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
        
        var friendship = await unitOfWork.Friendships.GetByUsersAsync(firstUser, secondUser, cancellationToken);
        if (friendship is null)
        {
            return new EntityNotFoundError(nameof(Friendship), "such users");
        }

        var confirmResult = friendship.ConfirmBy(secondUser);
        if (confirmResult.IsFailed)
        {
            return confirmResult;
        }
        
        unitOfWork.Friendships.Update(friendship);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new FriendshipDto(
            friendship.FirstUser.Id,
            friendship.SecondUser.Id,
            friendship.IsConfirmed);
    }
}