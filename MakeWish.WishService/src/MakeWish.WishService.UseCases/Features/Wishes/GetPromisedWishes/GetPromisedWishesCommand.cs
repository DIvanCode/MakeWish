using FluentResults;
using MakeWish.WishService.UseCases.Dto;
using MediatR;

namespace MakeWish.WishService.UseCases.Features.Wishes.GetPromisedWishes;

public sealed record GetPromisedWishesCommand : IRequest<Result<List<WishDto>>>;