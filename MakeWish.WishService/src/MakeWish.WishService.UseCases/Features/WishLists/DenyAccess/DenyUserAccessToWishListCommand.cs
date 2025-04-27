using FluentResults;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.WishLists.DenyAccess;

public sealed record DenyUserAccessToWishListCommand(Guid Id, Guid UserId) : IRequest<Result>;