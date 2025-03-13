using FluentResults;
using MakeWish.UserService.Interfaces.DataAccess;
using MakeWish.UserService.Models;
using MakeWish.UserService.UseCases.Dto;
using MakeWish.UserService.UseCases.Services;
using MakeWish.UserService.Utils.Errors;
using MediatR;

namespace MakeWish.UserService.UseCases.Features.Friendships.GetConfirmedFriendships;

public sealed class GetConfirmedFriendshipsHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    : IRequestHandler<GetConfirmedFriendshipsCommand, Result<List<FriendshipDto>>>
{
    public async Task<Result<List<FriendshipDto>>> Handle(GetConfirmedFriendshipsCommand request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated)
        {
            return new AuthenticationError();
        }
        
        if (request.UserId != userContext.UserId)
        {
            return new ForbiddenError(
                nameof(Friendship), 
                "get confirmed", 
                nameof(User.Id), 
                request.UserId);
        }
        
        var user = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return new EntityNotFoundError(nameof(User), nameof(User.Id), request.UserId);
        }

        var friendships = await unitOfWork.Friendships.GetConfirmedForUserAsync(user, cancellationToken);

        return friendships
            .Select(x => new FriendshipDto(
                x.FirstUser.Id,
                x.SecondUser.Id,
                x.IsConfirmed))
            .ToList();
    }
}