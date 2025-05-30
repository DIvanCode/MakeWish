using FluentResults;
using MediatR;

namespace MakeWish.WishService.UseCases.Abstractions.Features.WishLists;

public sealed record DenyUserAccessToWishListCommand(Guid Id, Guid UserId) : IRequest<Result>;