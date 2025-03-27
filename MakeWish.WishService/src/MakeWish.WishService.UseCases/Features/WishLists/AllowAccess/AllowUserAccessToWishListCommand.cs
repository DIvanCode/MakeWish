using FluentResults;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.WishLists.AllowAccess;

public sealed record AllowUserAccessToWishListCommand(Guid UserId, Guid WishListId) : IRequest<Result>;