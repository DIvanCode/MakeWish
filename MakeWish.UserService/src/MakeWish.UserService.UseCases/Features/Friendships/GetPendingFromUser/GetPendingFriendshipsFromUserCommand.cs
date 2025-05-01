using FluentResults;
using MakeWish.UserService.UseCases.Dto;
using MediatR;

namespace MakeWish.UserService.UseCases.Features.Friendships.GetPendingFromUser;

public sealed record GetPendingFriendshipsFromUserCommand(Guid UserId) : IRequest<Result<List<FriendshipDto>>>;