using MakeWish.UserService.Models.Entities;

namespace MakeWish.UserService.Models.Events;

public sealed record FriendshipRemovedEvent(Friendship Friendship) : DomainEvent;