using FluentResults;
using MediatR;

namespace MakeWish.UserService.UseCases.Features.Friendships.RemoveFriendship;

public sealed record RemoveFriendshipCommand(int FirstUserId, int SecondUserId) : IRequest<Result>;