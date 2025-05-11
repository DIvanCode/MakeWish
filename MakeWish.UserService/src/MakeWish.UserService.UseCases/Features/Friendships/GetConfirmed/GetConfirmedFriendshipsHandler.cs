using FluentResults;
using MakeWish.UserService.Interfaces.DataAccess;
using MakeWish.UserService.Models.Entities;
using MakeWish.UserService.UseCases.Dto;
using MakeWish.UserService.UseCases.Services;
using MakeWish.UserService.Utils.Errors;
using MediatR;

namespace MakeWish.UserService.UseCases.Features.Friendships.GetConfirmed;

public sealed class GetConfirmedFriendshipsHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    : IRequestHandler<GetConfirmedFriendshipsCommand, Result<List<FriendshipDto>>>
{
    public async Task<Result<List<FriendshipDto>>> Handle(GetConfirmedFriendshipsCommand request, CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated)
        {
            return new AuthenticationError();
        }
        
        var user = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return new EntityNotFoundError(nameof(User), nameof(User.Id), request.UserId);
        }

        var friendships = await unitOfWork.Friendships.GetConfirmedForUserAsync(user, cancellationToken);

        return friendships.Select(FriendshipDto.FromFriendship).ToList();
    }
}