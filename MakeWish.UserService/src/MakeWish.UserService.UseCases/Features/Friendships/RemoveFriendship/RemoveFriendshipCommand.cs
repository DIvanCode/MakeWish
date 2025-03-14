using FluentResults;
using MediatR;

namespace MakeWish.UserService.UseCases.Features.Friendships.RemoveFriendship;

public sealed record RemoveFriendshipCommand(Guid FirstUserId, Guid SecondUserId) : IRequest<Result>;