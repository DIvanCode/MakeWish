using FluentResults;
using MakeWish.WishService.UseCases.Dto;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.WishLists.Get;

public sealed record GetWishListCommand(Guid WishListId) : IRequest<Result<WishListDto>>;