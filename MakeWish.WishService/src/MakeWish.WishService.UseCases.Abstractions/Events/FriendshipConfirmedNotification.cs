using MediatR;

namespace MakeWish.WishService.UseCases.Abstractions.Events;

public sealed record FriendshipConfirmedNotification(Guid FirstUserId, Guid SecondUserId) : INotification;