using FluentResults;
using MakeWish.UserService.UseCases.Dto;
using MediatR;

namespace MakeWish.UserService.UseCases.Features.Friendships.GetConfirmedFriendships;

public sealed record GetConfirmedFriendshipsCommand(Guid UserId) : IRequest<Result<List<FriendshipDto>>>;