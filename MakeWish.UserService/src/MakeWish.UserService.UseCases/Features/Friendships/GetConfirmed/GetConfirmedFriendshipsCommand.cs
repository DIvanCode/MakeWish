using FluentResults;
using MakeWish.UserService.UseCases.Dto;
using MediatR;

namespace MakeWish.UserService.UseCases.Features.Friendships.GetConfirmed;

public sealed record GetConfirmedFriendshipsCommand(Guid UserId) : IRequest<Result<List<FriendshipDto>>>;