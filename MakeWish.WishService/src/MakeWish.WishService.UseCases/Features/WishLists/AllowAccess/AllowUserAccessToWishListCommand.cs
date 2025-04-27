using FluentResults;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.WishLists.AllowAccess;

public sealed record AllowUserAccessToWishListCommand(Guid Id, Guid UserId) : IRequest<Result>;