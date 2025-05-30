using MediatR;

namespace MakeWish.WishService.UseCases.Abstractions.Events;

public sealed record UserDeletedNotification(Guid Id) : INotification;