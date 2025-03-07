using FluentResults;
using MakeWish.UserService.UseCases.Dto;
using MediatR;

namespace MakeWish.UserService.UseCases.Features.Friendships.CreateFriendship;

public sealed record CreateFriendshipCommand(int FirstUserId, int SecondUserId) : IRequest<Result<FriendshipDto>>;