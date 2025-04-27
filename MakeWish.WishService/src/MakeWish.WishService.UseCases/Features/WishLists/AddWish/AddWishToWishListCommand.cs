using FluentResults;
using MakeWish.WishService.UseCases.Dto;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.WishLists.AddWish;

public sealed record AddWishToWishListCommand(Guid Id, Guid WishId) : IRequest<Result<WishListDto>>;