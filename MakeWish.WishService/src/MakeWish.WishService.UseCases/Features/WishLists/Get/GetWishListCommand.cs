using FluentResults;
using MakeWish.WishService.UseCases.Dto;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.WishLists.Get;

public sealed record GetWishListCommand(Guid Id) : IRequest<Result<WishListDto>>;