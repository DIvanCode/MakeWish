using FluentResults;
using MakeWish.UserService.Interfaces.DataAccess;
using MakeWish.UserService.Models;
using MakeWish.UserService.UseCases.Dto;
using MakeWish.UserService.UseCases.Services;
using MakeWish.UserService.Utils.Errors;
using MediatR;

namespace MakeWish.UserService.UseCases.Features.Friendships.CreateFriendship;

public sealed class CreateFriendshipHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    : IRequestHandler<CreateFriendshipCommand, Result<FriendshipDto>>
{
    public async Task<Result<FriendshipDto>> Handle(
        CreateFriendshipCommand request,
        CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated)
        {
            return new AuthenticationError();
        }
        
        if (request.FirstUserId != userContext.UserId)
        {
            return new ForbiddenError(
                nameof(Friendship), 
                "create", 
                nameof(User.Id), 
                request.FirstUserId);
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
        
        var hasFriendInvitationBetweenUsers = await unitOfWork.Friendships
            .HasBetweenUsersAsync(firstUser, secondUser, cancellationToken);
        if (hasFriendInvitationBetweenUsers)
        {
            return new EntityAlreadyExistsError(nameof(Friendship), "such users");
        }
        
        var createResult = Friendship.Create(firstUser, secondUser);
        if (createResult.IsFailed)
        {
            return createResult.ToResult<FriendshipDto>();
        }
        
        var friendship = createResult.Value;
        unitOfWork.Friendships.Add(friendship);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return FriendshipDto.FromFriendship(friendship);
    }
}