using MediatR;

namespace MakeWish.WishService.UseCases.Abstractions.Events;

public sealed record UserCreatedNotification(Guid Id, string Name, string Surname) : INotification;