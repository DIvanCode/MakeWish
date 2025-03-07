using FluentResults;
using MakeWish.UserService.UseCases.Dto;
using MediatR;

namespace MakeWish.UserService.UseCases.Features.Friendships.GetPendingFriendshipsFromUser;

public sealed record GetPendingFriendshipsFromUserCommand(int UserId) : IRequest<Result<List<FriendshipDto>>>;