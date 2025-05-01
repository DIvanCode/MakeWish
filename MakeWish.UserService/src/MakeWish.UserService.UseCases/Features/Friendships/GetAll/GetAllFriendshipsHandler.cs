using FluentResults;
using MakeWish.UserService.Interfaces.DataAccess;
using MakeWish.UserService.UseCases.Dto;
using MediatR;

namespace MakeWish.UserService.UseCases.Features.Friendships.GetAll;

public sealed class GetAllFriendshipsHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<GetAllFriendshipsCommand, Result<List<FriendshipDto>>>
{
    public async Task<Result<List<FriendshipDto>>> Handle(
        GetAllFriendshipsCommand request,
        CancellationToken cancellationToken)
    {
        var friendships = await unitOfWork.Friendships.GetAllConfirmedAsync(cancellationToken);
        return friendships.Select(FriendshipDto.FromFriendship).ToList();
    }
}