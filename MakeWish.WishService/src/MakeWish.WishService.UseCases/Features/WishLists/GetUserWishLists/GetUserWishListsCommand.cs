using FluentResults;
using MakeWish.WishService.UseCases.Dto;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.WishLists.GetUserWishLists;

public sealed record GetUserWishListsCommand(Guid UserId): IRequest<Result<List<WishListDto>>>;