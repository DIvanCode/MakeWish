using FluentResults;
using MakeWish.UserService.Interfaces.DataAccess;
using MakeWish.UserService.Models;
using MakeWish.UserService.UseCases.Dto;
using MakeWish.UserService.UseCases.Services;
using MakeWish.UserService.Utils.Errors;
using MediatR;

namespace MakeWish.UserService.UseCases.Features.Friendships.GetPendingFriendshipsFromUser;

public sealed class GetPendingFriendshipsFromUserHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    : IRequestHandler<GetPendingFriendshipsFromUserCommand, Result<List<FriendshipDto>>>
{
    public async Task<Result<List<FriendshipDto>>> Handle(
        GetPendingFriendshipsFromUserCommand request,
        CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated)
        {
            return new AuthenticationError();
        }
        
        if (request.UserId != userContext.UserId)
        {
            return new ForbiddenError(
                nameof(Friendship), 
                "get pending from user", 
                nameof(User.Id), 
                request.UserId);
        }
        
        var user = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return new EntityNotFoundError(nameof(User), nameof(User.Id), request.UserId);
        }

        var friendships = await unitOfWork.Friendships.GetPendingFromUserAsync(user, cancellationToken);

        return friendships.Select(FriendshipDto.FromFriendship) .ToList();
    }
}