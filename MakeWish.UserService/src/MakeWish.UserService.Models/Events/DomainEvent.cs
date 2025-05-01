using MediatR;

namespace MakeWish.UserService.Models.Events;

public abstract record DomainEvent : INotification;