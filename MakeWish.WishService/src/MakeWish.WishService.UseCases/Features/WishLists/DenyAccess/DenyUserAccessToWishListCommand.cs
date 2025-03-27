using FluentResults;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.WishLists.AllowReadCancel;

public sealed record DenyUserAccessToWishListCommand(Guid WishListId, Guid UserId) : IRequest<Result>;