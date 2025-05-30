using FluentResults;
using MakeWish.WishService.UseCases.Abstractions.Dto;
using MediatR;

namespace MakeWish.WishService.UseCases.Abstractions.Features.WishLists;

public sealed record GetUserWishListsCommand(Guid UserId): IRequest<Result<List<WishListDto>>>;