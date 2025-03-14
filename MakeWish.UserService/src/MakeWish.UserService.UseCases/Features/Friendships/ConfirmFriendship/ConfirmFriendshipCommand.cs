using FluentResults;
using MakeWish.UserService.UseCases.Dto;
using MediatR;

namespace MakeWish.UserService.UseCases.Features.Friendships.ConfirmFriendship;

public sealed record ConfirmFriendshipCommand(Guid FirstUserId, Guid SecondUserId)
    : IRequest<Result<FriendshipDto>>;