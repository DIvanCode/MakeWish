using FluentResults;
using MakeWish.WishService.UseCases.Abstractions.Dto;
using MediatR;

namespace MakeWish.WishService.UseCases.Abstractions.Features.WishLists;

public sealed record GetWishListCommand(Guid Id) : IRequest<Result<WishListDto>>;