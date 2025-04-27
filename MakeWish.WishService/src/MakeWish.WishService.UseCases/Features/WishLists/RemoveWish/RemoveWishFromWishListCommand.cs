using FluentResults;
using MakeWish.WishService.UseCases.Dto;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.WishLists.RemoveWish;

public sealed record RemoveWishFromWishListCommand(Guid Id, Guid WishId) : IRequest<Result<WishListDto>>;