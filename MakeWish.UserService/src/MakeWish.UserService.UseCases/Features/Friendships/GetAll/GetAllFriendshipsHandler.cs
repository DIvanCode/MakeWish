using FluentResults;
using MakeWish.UserService.Interfaces.DataAccess;
using MakeWish.UserService.UseCases.Dto;
using MakeWish.UserService.UseCases.Services;
using MakeWish.UserService.Utils.Errors;
using MediatR;

namespace MakeWish.UserService.UseCases.Features.Friendships.GetAll;

public sealed class GetAllFriendshipsHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    : IRequestHandler<GetAllFriendshipsCommand, Result<List<FriendshipDto>>>
{
    public async Task<Result<List<FriendshipDto>>> Handle(
        GetAllFriendshipsCommand request,
        CancellationToken cancellationToken)
    {
        if (!userContext.IsAdmin)
        {
            return new ForbiddenError("Cannot get all friendships");
        }
        
        var friendships = await unitOfWork.Friendships.GetAllConfirmedAsync(cancellationToken);
        return friendships.Select(FriendshipDto.FromFriendship).ToList();
    }
}