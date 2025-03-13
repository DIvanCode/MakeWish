using FluentResults;
using MakeWish.UserService.UseCases.Dto;
using MediatR;

namespace MakeWish.UserService.UseCases.Features.Friendships.GetPendingFriendshipsToUser;

public sealed record GetPendingFriendshipsToUserCommand(Guid UserId) : IRequest<Result<List<FriendshipDto>>>;