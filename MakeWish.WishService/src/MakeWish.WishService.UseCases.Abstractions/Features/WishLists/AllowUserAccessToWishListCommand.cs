using FluentResults;
using MediatR;

namespace MakeWish.WishService.UseCases.Abstractions.Features.WishLists;

public sealed record AllowUserAccessToWishListCommand(Guid Id, Guid UserId) : IRequest<Result>;