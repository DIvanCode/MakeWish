using MediatR;

namespace MakeWish.WishService.UseCases.Abstractions.Events;

public sealed record FriendshipRemovedNotification(Guid FirstUserId, Guid SecondUserId) : INotification;