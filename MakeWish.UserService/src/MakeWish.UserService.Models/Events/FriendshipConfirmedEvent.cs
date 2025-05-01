using MakeWish.UserService.Models.Entities;

namespace MakeWish.UserService.Models.Events;

public sealed record FriendshipConfirmedEvent(Friendship Friendship) : DomainEvent;